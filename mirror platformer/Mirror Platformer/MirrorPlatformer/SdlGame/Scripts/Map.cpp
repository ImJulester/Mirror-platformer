#include "Map.h"
#include "Game.h"
#include <fstream>
#include <iostream>


Map::Map()
{

}

Map::~Map()
{
}

void Map::LoadMap(std::string path)
{
	std::cout << " hello? " << std::endl;
	int sizeX;
	int sizeY;
	char c;
	std::fstream mapdata;
	mapdata.open(path + "i.map");

	while (mapdata >> sizeX >> sizeY)
	{
		std::cout << sizeX << sizeY << std::endl;
	}
	mapdata.close();

	std::cout << sizeX << sizeY << std::endl;

	std::fstream mapFile;
	mapFile.open(path + ".map");

	int srcX, srcY;

	for (int y = 0; y < sizeY; y++)
	{
		for (int x = 0; x < sizeX; x++)
		{
			mapFile.get(c);
			srcY = atoi(&c) * 32;
			mapFile.get(c);
			srcX = atoi(&c) * 32;
			Game::AddTile(srcX, srcY, x * 32, y * 32, atoi(&c));
			mapFile.ignore();
		}
	}

	mapFile.close();
}

void Map::LoadMap(std::string path, int sizeX, int sizeY)
{
	char c;
	std::fstream mapFile;
	mapFile.open(path);

	int srcX, srcY;

	for (int y = 0; y < sizeY; y++)
	{
		for (int x = 0; x < sizeX; x++)
		{
			mapFile.get(c);
			srcY = atoi(&c) * 32;
			mapFile.get(c);
			srcX = atoi(&c) * 32;
			Game::AddTile(srcX, srcY, x * 32, y * 32, atoi(&c));
			mapFile.ignore();
		}
	}

	mapFile.close();
}

void Map::LoadMap(std::string path, bool reload)
{
	std::cout << " hello? " << std::endl;
	int sizeX;
	int sizeY;
	char c;
	std::fstream mapdata;
	mapdata.open(path + "i.map");

	while (mapdata >> sizeX >> sizeY)
	{
		std::cout << sizeX << sizeY << std::endl;
	}
	mapdata.close();


	std::fstream mapFile;
	mapFile.open(path + ".map");

	int srcX, srcY;

	for (int y = 0; y < sizeY; y++)
	{
		for (int x = 0; x < sizeX; x++)
		{
			mapFile.get(c);
			srcY = atoi(&c) * 32;
			mapFile.get(c);
			srcX = atoi(&c) * 32;
			Game::AddTile(srcX, srcY, x * 32, y * 32, atoi(&c));
			mapFile.ignore();
		}
	}
	Game::setTransition(reload);
	mapFile.close();
}

void Map::LoadMap(std::string path, int sizeX, int sizeY,bool reload)
{
	char c;
	std::fstream mapFile;
	mapFile.open(path);

	int srcX, srcY;

	for (int y = 0; y < sizeY; y++)
	{
		for (int x = 0; x < sizeX; x++)
		{
			mapFile.get(c);
			srcY = atoi(&c) * 32;
			mapFile.get(c);
			srcX = atoi(&c) * 32;
			Game::AddTile(srcX, srcY, x * 32, y * 32, atoi(&c));
			mapFile.ignore();
		}
	}
	Game::setTransition(reload);
	mapFile.close();
}

