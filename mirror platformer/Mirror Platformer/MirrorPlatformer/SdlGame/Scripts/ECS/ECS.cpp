#include "ECS.h"

void Entity::addGroup(Group g) 
{
	groupBitSet[g] = true;
	manager.AddToGroup(this, g);
}