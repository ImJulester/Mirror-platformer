#pragma once
#include <string>
#include "SDL.h"
#include "Components.h"

class ColliderComponent :public Component
{
public:
	SDL_Rect collider;
	std::string tag;
	int id;
	int height;
	int width;
	int xoffset;
	int yoffset;

	bool customSize;
	TransformComponent* transform;

	ColliderComponent(std::string t)
	{
		customSize = false;
		tag = t;
		id = 0;
	}

	ColliderComponent(std::string t, int w, int h, int xoff, int yoff)
	{
		customSize = true;
		height = h;
		width = w;
		xoffset = xoff;
		yoffset = yoff;
		tag = t;
		id = 0;
	}

	ColliderComponent(std::string t, int i, int w, int h, int xoff, int yoff)
	{
		customSize = true;
		height = h;
		width = w;
		xoffset = xoff;
		yoffset = yoff;
		tag = t;
		id = i;
	}
	ColliderComponent(std::string t, int ID)
	{
		customSize = false;
		tag = t;
		id = ID + 1;
	}

	void init() override
	{

		transform = &entity->getComponent<TransformComponent>();


		Game::colliders.push_back(this);
	}


	void UpdateCollisionRect()
	{
		collider.x = static_cast<int>(transform->position.x + xoffset);
		collider.y = static_cast<int>(transform->position.y + yoffset);
		collider.w = width * transform->scale;
		collider.h = height * transform->scale;
	}

	void Update() override
	{
		//Custom size if sprite size is not suitable
		//set a xoffset and yoffset and a matching width / height to get the desired size
			if (customSize)
			{
				collider.x = static_cast<int>(transform->position.x + xoffset);
				collider.y = static_cast<int>(transform->position.y + yoffset);
				collider.w = width * transform->scale;
				collider.h = height * transform->scale;
			}
			else
			{
				collider.x = static_cast<int>(transform->position.x);
				collider.y = static_cast<int>(transform->position.y);
				collider.w = transform->width * transform->scale;
				collider.h = transform->height * transform->scale;
			}
		
	}
};