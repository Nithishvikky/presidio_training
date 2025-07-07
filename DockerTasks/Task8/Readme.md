docker service scale nginx-web=5

docker service ls

ID             NAME        MODE         REPLICAS   IMAGE          PORTS
1aedq81w6rg3   nginx-web   replicated   5/5        nginx:alpine   *:8080->80/tcp