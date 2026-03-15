#!/bin/bash
COMPOSE_FILE="docker-compose/docker-compose.yml"

echo "Starting Docker test environment..."
docker-compose -f $COMPOSE_FILE up -d

echo "Waiting for services to become healthy..."
sleep 10

echo "Pinging localhost:5001..."
curl -kI https://localhost:5001

echo "Tearing down Docker test environment..."
docker-compose -f $COMPOSE_FILE down
