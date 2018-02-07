#pragma once
#include <string>

class Map
{

public:
	Map();
	~Map();


	static void LoadMap(std::string path);
	static void LoadMap(std::string path, int sizeX, int sizeY);
	static void LoadMap(std::string path, int sizeX, int sizeY,bool reload);
	static void LoadMap(std::string path, bool reload);


private:

};