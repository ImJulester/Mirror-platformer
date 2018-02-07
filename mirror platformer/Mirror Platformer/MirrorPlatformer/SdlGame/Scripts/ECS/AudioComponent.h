#pragma once
#include "SDL.h"
#include "Components.h"
#include <SDL_mixer.h>
class AudioComponent : public Component
{

private:
	Mix_Chunk *sound = NULL;
	const char* soundPath;

public:
	AudioComponent() = default;

	AudioComponent(const char* path)
	{
		sound = LoadSound(path);
		soundPath = path;
	}

	AudioComponent(const char* path,int volume)
	{
		sound = LoadSound(path);
		soundPath = path;
		SetVolume(volume* 10);
	}

	void SetVolume(int v) 
	{
		Mix_Volume(-1, v);
	}

	void Load(const char* path)
	{
		sound = LoadSound(path);
	}

	Mix_Chunk* LoadSound(const char* path)
	{
		return Mix_LoadWAV(path);
	}

	void Play()
	{
		Mix_PlayChannel(-1, sound, 0);
	}

	void PlayOnce(const char* path)
	{
		sound = LoadSound(path);
		Mix_PlayChannel(-1, sound, 0);
		sound = LoadSound(soundPath);
	}

	void LoadAndPLay(const char* path)
	{
		sound = LoadSound(path);
		Mix_PlayChannel(-1, sound, 0);
	}




};