#pragma once

#include "SDL.h"
#include <iostream>
#include "SDL_image.h"
#include <vector>

class ColliderComponent;

class Game
{
public:

	Game();
	~Game();
	void init(const char* title, int xpos, int ypos, int width, int height, bool fullscreen);


	void MirrorSpriteFlip();
	void handleEvents();
	void update();
	void render();
	void clean();
	bool running() { return isRunning; }
	static void setTransition(bool b);
	static void AddTile(int srcX, int srcY, int xpos, int ypos, int id);

	static SDL_Renderer *renderer;
	static SDL_Event event;
	static SDL_Rect camera;
	static std::vector<ColliderComponent*> colliders;


private:
	int cnt = 0;
	bool isRunning;
	bool mirrored;
	SDL_Window *window;

};