#include "Game.h"
#include "TextureManager.h"
#include "Map.h"
#include "ECS\Components.h"
#include "ECS\Vector2D.h"
#include  "Collision.h"

Map* map;
Manager manager;

SDL_Renderer* Game::renderer = nullptr;
SDL_Event Game::event;
SDL_Rect Game::camera = { 0,0,800,640 };

Vector2D startpos;

Mix_Music *music = NULL;

Mix_Chunk *jump = NULL;


int dieTimer = 0;

bool pause;

bool xHit;
bool yHit;

bool levelTransition;

std::vector<std::string> maplist;
int maplistIndex = 0;

std::vector<ColliderComponent*> Game::colliders;

auto& dieMessage(manager.addEntity());
auto& controls(manager.addEntity());
auto& player(manager.addEntity());



const char* mapfile = "assets/maptiles.png";
enum groupLabels : std::size_t
{
	groupMap,
	groupPlayer,
	groupEnemies,
	groupColliders,
	groupTutorial,
	groupDeadScreen
};



Game::Game() 
{

}
Game::~Game() 
{

}


void Game::init(const char *title, int xpos, int ypos, int width, int height, bool fullscreen) 
{
	int flags = 0;

	if (fullscreen) 
	{
		flags = SDL_WINDOW_FULLSCREEN;
	}

	if (SDL_Init(SDL_INIT_EVERYTHING) == 0)
	{
		std::cout << "subsystems initialised" << std::endl;

		window = SDL_CreateWindow(title,SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, flags);
		if (window) 
		{
			std::cout<< "window created" << std::endl;
		}

		renderer = SDL_CreateRenderer(window, -1, 0);
		if (renderer) 
		{
			SDL_SetRenderDrawColor(renderer, 255, 69, 0, 255);
			std::cout << "renderer created" << std::endl;
		}

		isRunning = true;
	}

	if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) == 0)
	{
		std::cout << " audio initialised" << std::endl;
	}

	if (Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 2, 2048) < 0)
	{
		std::cout << "Cant open audio" << std::endl;
	}
	
	// Load soundtrack
	music = Mix_LoadMUS("assets/sound/backgroundmusic.mp3");
	if (music == NULL)
	{
		std::cout << Mix_GetError() << std::endl;;
		std::cout << "Failed to load music!" << std::endl;
	}

	Mix_VolumeMusic(MIX_MAX_VOLUME / 10);
	Mix_PlayMusic(music, -1);

	//Add some components to standart entities
	dieMessage.addComponent<TransformComponent>(100, 100, 146, 81, 1);
	dieMessage.addComponent<SpriteComponent>("assets/died.png");
	dieMessage.addGroup(groupDeadScreen);


	controls.addComponent<TransformComponent>(100,100,146,81,1);
	controls.addComponent<SpriteComponent>("assets/controls.png");
	controls.addGroup(groupTutorial);

	maplist.push_back("assets/map/bigmap");
	maplist.push_back("assets/map/bigmap3");
	maplist.push_back("assets/map/bigmap2");

	Map::LoadMap(maplist.at(maplistIndex));
	maplistIndex++;

	player.addComponent<TransformComponent>(20,100,32,32,2,true);
	player.addComponent<SpriteComponent>("assets/BaseCharacterWalknormal.png", true);
	player.addComponent<AudioComponent>("assets/sound/jump_07.wav", 5);
	player.addComponent<KeyboardController>(); 
	player.addComponent<ColliderComponent>("player",10,24,13,5);
	startpos = player.getComponent<TransformComponent>().position;
	player.addGroup(groupPlayer);

	pause = false;
	mirrored = false;
}


void Game::handleEvents() 
{
	SDL_PollEvent(&event);
	switch (event.type) 
	{	
	case SDL_QUIT:
		isRunning = false;
		break;
	default:
		break;
	}
}

void Game::update() 
{
	if (levelTransition)
	{
		return;
	}

	if (pause) 
	{
		return;
	}
	//Time till respawn in frames 
	if (dieTimer >=0) 
	{
		dieTimer--;

		//Last frame of timer
		if (dieTimer == 0) 
		{
			player.getComponent<TransformComponent>().position = startpos;
			player.getComponent<TransformComponent>().velocity.Zero();
			player.getComponent<SpriteComponent>().spriteFlip = SDL_FLIP_NONE;
			if (player.getComponent<KeyboardController>().mirrored) 
			{
				player.getComponent<SpriteComponent>().SetTex("assets/BaseCharacterWalknormal.png");
				player.getComponent<KeyboardController>().GravityScale = 1;
				player.getComponent<KeyboardController>().mirrored = false;
			}


		}
		return;
	}


	//Reset collision check bools
	xHit = false;
	yHit = false;
	


	//Camera movment inside screen 
	camera.x = player.getComponent<TransformComponent>().position.x - 400;

	if (camera.x < 0) 
	{
		camera.x = 0;
	}
	if (camera.x > 2400) 
	{
		camera.x = 2400;
	}




	//Apply gravity to player
	if (!player.getComponent<KeyboardController>().grounded)
	{
		player.getComponent<TransformComponent>().velocity.y += 0.05f * player.getComponent<KeyboardController>().GravityScale;
	}



	

	///////////////////////////////////////////
	//Move player and player collider on y axis
	///////////////////////////////////////////
	player.getComponent<TransformComponent>().position.y += player.getComponent<TransformComponent>().velocity.y * player.getComponent<TransformComponent>().speed;
	player.getComponent<ColliderComponent>().UpdateCollisionRect();

	for (auto cc : colliders)
	{
		if (Collision::AABB(player.getComponent<ColliderComponent>(), *cc))
		{
			//First checking for special collisions

			//Hit spike = dead
			if (cc->id == 1 || cc->id == 5 || cc->id == 7 || cc->id == 8)
			{
				player.getComponent<AudioComponent>().PlayOnce("assets/sound/Hit_Hurt10.wav");
				dieTimer = 150;
				dieMessage.getComponent<TransformComponent>().position.y = player.getComponent<TransformComponent>().position.y - (100 * player.getComponent<KeyboardController>().GravityScale);
				dieMessage.getComponent<TransformComponent>().position.x = player.getComponent<TransformComponent>().position.x;
			}


			//end lvl check
			if (cc->id == 9)
			{
				levelTransition = true;
				//Destroy old map
				for (auto t : manager.getGroup(groupMap))
				{
				t->Destroy();
				}

				//remove old colliders
				colliders.clear();

				//Load new map
				Map::LoadMap(maplist.at(maplistIndex),false);

				//Set player start position
				player.getComponent<TransformComponent>().position = startpos;
				player.getComponent<KeyboardController>().mirrored = false;
				player.getComponent<SpriteComponent>().SetTex("assets/BaseCharacterWalknormal.png");
				player.getComponent<SpriteComponent>().spriteFlip = SDL_FLIP_NONE;
				player.getComponent<KeyboardController>().GravityScale = 1;
				player.getComponent<KeyboardController>().grounded = false;
				player.getComponent<KeyboardController>().inair = true;
				player.getComponent<TransformComponent>().velocity.Zero();
				return;
			}

			//Hit mirror while normal
			if(cc->id == 3 && !player.getComponent<KeyboardController>().mirrored)
			{
				player.getComponent<AudioComponent>().PlayOnce("assets/sound/PortalEnter.wav");
				
				player.getComponent<SpriteComponent>().SetTex("assets/BaseCharacterWalkmirror.png");
				player.getComponent<KeyboardController>().GravityScale = -1;
				player.getComponent<TransformComponent>().position.y += 50;

				//Add enough velocity to get out of portal. 
				if (player.getComponent<TransformComponent>().velocity.y < 1.1f) 
				{
					player.getComponent<TransformComponent>().velocity.y = 1.2f;
				}

				player.getComponent<KeyboardController>().mirrored = true;
				MirrorSpriteFlip();
				return;
			}
			//Hit mirror upsidedown
			else
			if (cc->id == 4 && player.getComponent<KeyboardController>().mirrored)
			{
				
				
				player.getComponent<AudioComponent>().PlayOnce("assets/sound/PortalExit.wav");
				player.getComponent<SpriteComponent>().SetTex("assets/BaseCharacterWalknormal.png");
				player.getComponent<KeyboardController>().GravityScale = 1;
				player.getComponent<TransformComponent>().position.y -= 50;

				//Add enough velocity to get out of portal. 
				if (player.getComponent<TransformComponent>().velocity.y > -1.1f)
				{
					player.getComponent<TransformComponent>().velocity.y = -1.2f;
				}

				player.getComponent<KeyboardController>().mirrored = false;
				MirrorSpriteFlip();
				return;
			}


			//If collision with black ground tile and not upsidedown
			if (!player.getComponent<KeyboardController>().mirrored) 
			{
				if (cc->id == 2)
				{

					if (player.getComponent<KeyboardController>().inair)
					{
						if (cc->entity->getComponent<TransformComponent>().position.y > player.getComponent<TransformComponent>().position.y) 
						{
							player.getComponent<KeyboardController>().grounded = true;
							if (player.getComponent<TransformComponent>().velocity.x == 0) 
							{
								player.getComponent<SpriteComponent>().Play("idle");
							}
							else 
							{
								player.getComponent<SpriteComponent>().Play("walk");
							}
							player.getComponent<KeyboardController>().inair = false;
						}
						else 
						{
							player.getComponent<TransformComponent>().velocity.y = 0.5f;
							player.getComponent<TransformComponent>().position.y += 1;
						}


					}
					yHit = true;

				}
			}
			else 
			{
				//If collision with white ground tile and upsidedown
				if (cc->id == 6)
				{
					if (player.getComponent<KeyboardController>().inair)
					{
						if (cc->entity->getComponent<TransformComponent>().position.y < player.getComponent<TransformComponent>().position.y)
						{
							if (player.getComponent<TransformComponent>().velocity.x == 0)
							{
								player.getComponent<SpriteComponent>().Play("idle");
							}
							else
							{
								player.getComponent<SpriteComponent>().Play("walk");
							}
							player.getComponent<KeyboardController>().grounded = true;
							player.getComponent<KeyboardController>().inair = false;
						}
						else
						{
							player.getComponent<TransformComponent>().velocity.y = -0.5f;
							player.getComponent<TransformComponent>().position.y -= 1;
						}
					}
					yHit = true;

				}
			}

		}
	}

	//Collision on y axis
	//Return to previous position
	if (player.getComponent<TransformComponent>().position.y < -50) 
	{
		player.getComponent<TransformComponent>().position.y -= player.getComponent<TransformComponent>().velocity.y * player.getComponent<TransformComponent>().speed;
		player.getComponent<TransformComponent>().velocity.y = 0.5f;
	}
	if (player.getComponent<TransformComponent>().position.y > 610) 
	{
		player.getComponent<TransformComponent>().position.y -= player.getComponent<TransformComponent>().velocity.y * player.getComponent<TransformComponent>().speed;
		player.getComponent<TransformComponent>().velocity.y = -0.5f;
	}

	if (yHit)
	{
		player.getComponent<TransformComponent>().position.y -= player.getComponent<TransformComponent>().velocity.y * player.getComponent<TransformComponent>().speed ;
	}
	else
	{
		if (player.getComponent<KeyboardController>().grounded) 
		{
			player.getComponent<TransformComponent>().velocity.y = 0;
		}

		player.getComponent<KeyboardController>().inair = true;
		player.getComponent<KeyboardController>().grounded = false;

	}

	////////////////////////////////////
	//Move player and collider on x axis
	////////////////////////////////////
	player.getComponent<TransformComponent>().position.x += player.getComponent<TransformComponent>().velocity.x * player.getComponent<TransformComponent>().speed;
	player.getComponent<ColliderComponent>().UpdateCollisionRect();

	for (auto cc : colliders)
	{
		//check collision after moving
		if (Collision::AABB(player.getComponent<ColliderComponent>(), *cc))	
		{
			if (!player.getComponent<KeyboardController>().mirrored)
			{
				if (cc->id == 2)
				{
					xHit = true;
				}
			}
			else 
			{
				if (cc->id == 6)
				{
					xHit = true;
				}
			}
		}
	}

	//Collision on x axis
	//Return to previous position
	if (xHit) 
	{
		player.getComponent<TransformComponent>().position.x -= player.getComponent<TransformComponent>().velocity.x * player.getComponent<TransformComponent>().speed;
	}

	


	//Refresh Entities
	manager.Refresh();

	//Update all the components in entities
	manager.Update();

	

}




auto& tiles(manager.getGroup(groupMap));
auto& players(manager.getGroup(groupPlayer));
auto& enemies(manager.getGroup(groupEnemies));
auto& tutorial(manager.getGroup(groupTutorial));
auto& Deadscreen(manager.getGroup(groupDeadScreen));

void Game::render() 
{
	SDL_RenderClear(renderer);
	//Add stuff to renderer
	for (auto& t : tiles) 
	{
		t->Draw();
	}
	for (auto& p : players)
	{
		p->Draw();
	}
	for (auto& e : enemies)
	{
		e->Draw();
	}
	for (auto& tu : tutorial)
	{
		tu->Draw();
	}

	for (auto& ds : Deadscreen)
	{
		if (dieTimer >= 0)
		{
			ds->Draw();
		}
	}
	
	SDL_RenderPresent(renderer);
}
void Game::clean() 
{
	SDL_DestroyWindow(window);
	SDL_DestroyRenderer(renderer);
	Mix_FreeChunk(jump);
	Mix_FreeMusic(music);
	Mix_CloseAudio();
	SDL_Quit();
	std::cout << " game cleaned " << std::endl;

}

void Game::setTransition(bool b)
{
	levelTransition = b;
}

void Game::AddTile(int srcX, int srcY, int xpos, int ypos, int id)
{
	auto& tile(manager.addEntity());
	tile.addComponent<TileComponent>(srcX, srcY, xpos, ypos, mapfile);
	tile.addComponent<TransformComponent>(xpos, ypos);
	tile.addComponent<ColliderComponent>("tile", id);
	tile.addGroup(groupMap);
}


//vertical flip while keeping original horizontal flip
void Game::MirrorSpriteFlip()
{

	if (player.getComponent<KeyboardController>().mirrored == true)
	{
		if (player.getComponent<SpriteComponent>().spriteFlip == SDL_FLIP_HORIZONTAL)
		{
			player.getComponent<SpriteComponent>().spriteFlip = (SDL_RendererFlip)(SDL_FLIP_HORIZONTAL | SDL_FLIP_VERTICAL);
		}
		else
		{
			player.getComponent<SpriteComponent>().spriteFlip = SDL_FLIP_VERTICAL;
		}
	}
	else
	{
		if (player.getComponent<SpriteComponent>().spriteFlip == (SDL_RendererFlip)(SDL_FLIP_HORIZONTAL | SDL_FLIP_VERTICAL))
		{
			player.getComponent<SpriteComponent>().spriteFlip = SDL_FLIP_HORIZONTAL;
		}
		else
		{
			player.getComponent<SpriteComponent>().spriteFlip = SDL_FLIP_NONE;
		}
	}
}





