	#pragma once

#include "Components.h"
#include "Vector2D.h"

class TransformComponent : public Component
{


public:

	bool manageMovement;

	Vector2D position;
	Vector2D velocity;

	int height = 32;
	int width = 32;
	int scale = 1;

	float speed = 3;

	TransformComponent()
	{
		position.Zero();
	}
	TransformComponent(float x, float y)
	{
		position.x = x;
		position.y = y;
		manageMovement = false;
	}

	TransformComponent(float x, float y,float s)
	{
		position.x = x;
		scale = s;
		position.y = y;
		manageMovement = false;
	}

	TransformComponent(int sc) 
	{
		position.Zero();
		scale = sc;
		manageMovement = false;
	}

	TransformComponent(float x, float y, int w, int h,int sc)
	{
		position.x = x;
		position.y = y;
		width = w;
		height = h;
		scale = sc;
		manageMovement = false;
	}

	TransformComponent(float x, float y, int w, int h, int sc, bool customMovement)
	{
		position.x = x;
		position.y = y;
		width = w;
		height = h;
		scale = sc;
		manageMovement = customMovement;
	}

	void init() override 
	{
		velocity.Zero(); 
	}

	void SetScale(int x, int y) 
	{

	}

	void Update() override
	{

		if (!manageMovement)
		{
			position.x += velocity.x * speed;
			position.y += velocity.y * speed;
		}

	}

};