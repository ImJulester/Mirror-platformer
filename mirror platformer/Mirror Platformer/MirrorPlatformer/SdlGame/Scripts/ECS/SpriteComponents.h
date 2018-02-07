#pragma once

#include "Components.h"
#include "SDL.h"
#include "../TextureManager.h"
#include "Animation.h"
#include "game.h"
#include <map>


class SpriteComponent : public Component
{
private:
	TransformComponent* transform;
	SDL_Texture *texture;
	SDL_Rect srcRect, destRect, cameraOffset;

	bool animated = false;
	int frames = 0;
	int speed = 100;
public:
	
	int animIndex = 0;
	std::map < const char*, Animation > animations;

	SDL_RendererFlip spriteFlip = SDL_FLIP_NONE; 

	SpriteComponent() = default;

	SpriteComponent(const char* path)
	{
		SetTex(path);
	}

	SpriteComponent(const char* path,bool isAnimated)
	{
		
		animated = isAnimated;
		//Animation(index,Frames,Speed)
		Animation walk = Animation(1, 6, 200);
		Animation idle = Animation(0, 1, 200);
		Animation jump = Animation(2, 1, 200);
		animations.emplace("jump", jump);
		animations.emplace("idle", idle);
		animations.emplace("walk", walk);

		Play("idle");
		SetTex(path);
	}

	~SpriteComponent()
	{
		SDL_DestroyTexture(texture);
	}


	void SetTex(const char* path) 
	{
		texture = TextureManager::LoadTexture(path);
	}

	void init() override 
	{
		transform = &entity->getComponent<TransformComponent>();
		
		srcRect.x = 0;
		srcRect.y = 0;
		srcRect.w = transform->width;
		srcRect.h = transform->height;
	}

	void Update() override 
	{

		if (animated) 
		{
			srcRect.x = srcRect.w * static_cast<int>((SDL_GetTicks() / speed) % frames);
		}

		srcRect.y = animIndex * transform->height;

		destRect.x = static_cast<int>(transform->position.x) - Game::camera.x;
		destRect.y = static_cast<int>(transform->position.y) - Game::camera.y;
		
		destRect.w = transform->width * transform->scale;
		destRect.h = transform->height * transform->scale;
	}

	void Draw() override 
	{
		TextureManager::Draw(texture, srcRect, destRect, spriteFlip);
	}

	void Play(const char* animName) 
	{
		frames = animations[animName].frames;
		animIndex = animations[animName].index;
		speed = animations[animName].speed;
	}
};