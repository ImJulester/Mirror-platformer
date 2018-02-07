#pragma once

#include "../game.h"
#include "ECS.h"
#include "Components.h"
#include "AudioComponent.h"
class KeyboardController : public Component 
{
public: 
	int GravityScale = 1;
	//AudioComponent * sound;
	TransformComponent *transform;
	SpriteComponent *sprite;
	bool grounded;
	bool inair;
	bool mirrored;
	bool left;
	void init() override 
	{
		
		transform = &entity->getComponent<TransformComponent>();
		sprite = &entity->getComponent<SpriteComponent>();
		inair = true;
	}

	void Update() override 
	{
		
		if (!grounded)
		{
			sprite->Play("jump");
		}

		if (Game::event.type == SDL_KEYDOWN)
		{
			//If pressed "W" jump
			if (Game::event.key.keysym.sym == SDLK_w && grounded)
			{
				entity->getComponent<AudioComponent>().Play();
				transform->position.y -= 10 * GravityScale; 
				transform->velocity.y = -2.0f * GravityScale;
				grounded = false;
				inair = true;
				//JUMPANIM
			}

			//Horizontal movement
			switch (Game::event.key.keysym.sym)
			{
			case SDLK_a:
				transform->velocity.x = -1;
				if (grounded) 
				{
					sprite->Play("walk");
				}
				if (GravityScale < 0) 
				{
					sprite->spriteFlip = (SDL_RendererFlip)(SDL_FLIP_HORIZONTAL | SDL_FLIP_VERTICAL);
				}
				else 
				{
					sprite->spriteFlip = SDL_FLIP_HORIZONTAL;
				}

				break;
			case SDLK_d:
				transform->velocity.x = 1;
				if (grounded)
				{
					sprite->Play("walk");
				}
				if (GravityScale < 0)
				{
					sprite->spriteFlip = (SDL_RendererFlip)(SDL_FLIP_NONE | SDL_FLIP_VERTICAL);
				}
				else
				{
					sprite->spriteFlip = SDL_FLIP_NONE;
				}

				break;
			default:
				break;
			}
		}

		//release to idle
		if (Game::event.type == SDL_KEYUP)
		{
			switch (Game::event.key.keysym.sym)
			{
			case SDLK_a:
				transform->velocity.x = 0;
				if (grounded) 
				{
					sprite->Play("idle");
				}

				break;
			case SDLK_d:
				transform->velocity.x = 0;
				if (grounded)
				{
					sprite->Play("idle");
				}
				break;
			default:
				break;
			}
		}


	}

		


};