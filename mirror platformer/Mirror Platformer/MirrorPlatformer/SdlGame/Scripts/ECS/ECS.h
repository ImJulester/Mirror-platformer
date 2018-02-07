#pragma once

#include <vector>
#include <memory>
#include <algorithm>
#include <bitset>
#include <array>

class Component;
class Entity;
class Manager;

using ComponentID = std::size_t;
using Group = std::size_t;
inline ComponentID GetNewComponentTypeID()
{
	static ComponentID lastID = 0u;
	return lastID++;
}

template <typename T> inline ComponentID GetComponentTypeID() noexcept 
{
	static ComponentID typeID = GetNewComponentTypeID();
	return typeID;
}

constexpr std::size_t maxComponents = 32;
constexpr std::size_t maxgroups = 32;
using ComponentBitSet = std::bitset<maxComponents>;
using GroupBitSet = std::bitset<maxgroups>;	
using ComponentArray = std::array < Component*, maxComponents>;

class Component 
{
public:
	Entity* entity;

	virtual void init() {}
	virtual void Update() {}
	virtual void Draw() {}

	virtual  ~Component() {}
};

class Entity 
{
private : 
	Manager& manager;
	bool active = true;
	std::vector< std::unique_ptr<Component>> components;

	ComponentArray componentArray;
	ComponentBitSet componentBitSet;	
	GroupBitSet groupBitSet;

public : 
	Entity(Manager& mManager) : manager(mManager) {}
	void Update()
	{
		for (auto&c : components) c->Update();
	}
	void Draw() 
	{

			for (auto&c : components) c->Draw();

	}

	bool isActive() const
	{
		return active;
	}

	void Destroy() 
	{
		active = false;
	}

	void Enable() 
	{
		active = true;
	}
	bool hasGroup(Group g) 
	{
		return groupBitSet[g];
	}

	void addGroup(Group g);

	void delGroup(Group g) 
	{
		groupBitSet[g] = false;
	}

	template <typename T> bool hasComponent() const 
	{
		return ComponentBitSet[GetComponentID<T>];
	}


	template <typename T, typename... TArgs>
	T& addComponent(TArgs&&... mArgs) 
	{
		T* c(new T(std::forward<TArgs>(mArgs)...));
		c->entity = this;
		std::unique_ptr<Component> uPtr{ c }; 
		components.emplace_back(std::move(uPtr));

		componentArray[GetComponentTypeID<T>()] = c;
		componentBitSet[GetComponentTypeID<T>()] = true;
		
		c->init();
		return *c;
	
	}

	template<typename T> T& getComponent() const
	{
		auto ptr(componentArray[GetComponentTypeID<T>()]);
		return *static_cast<T*>(ptr);
	}
	

};

class Manager 
{
private: 
	std::vector<std::unique_ptr<Entity>> entities;
	std::array<std::vector<Entity*>, maxgroups> groupedEntities;
public:
	void Update() 
	{
		for (auto& e : entities) e->Update();
	}

	void Draw() 
	{
		for (auto& e : entities) e->Draw();
	}

	void Refresh() 
	{
		for (auto i(0); i < maxgroups; i++) 
		{
			auto& v(groupedEntities[i]);
			v.erase(std::remove_if(std::begin(v), std::end(v),
				[i](Entity* mEntity)
			{
				return !mEntity->isActive() || !mEntity->hasGroup(i);
			}),
				std::end(v));
		}

		entities.erase(std::remove_if(std::begin(entities), std::end(entities),
			[](const std::unique_ptr<Entity> & mEntity)
		{
			return !mEntity->isActive();
		}),
			std::end(entities));
	}

	void AddToGroup(Entity* mEntity, Group mGroup) 
	{
		groupedEntities[mGroup].emplace_back(mEntity);
	}

	std::vector<Entity*>& getGroup(Group mGroup) 
	{
		return groupedEntities[mGroup];
	}

	Entity& addEntity() 
	{
		Entity *e = new Entity(*this);
		std::unique_ptr<Entity> uPtr{ e };
		entities.emplace_back(std::move(uPtr));
		return *e;
	}

};
