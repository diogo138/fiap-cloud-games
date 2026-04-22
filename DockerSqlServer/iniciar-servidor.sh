#!/bin/bash
docker network prune -f
docker compose -f docker-compose.linux.yml up -d
