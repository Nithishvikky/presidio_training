docker swarm init
docker service create \
--name nginx-web \
 --replicas 3 \
 -p 8080:80 \
nginx

- --name nginx-web: service name.
- --replicas 3: run 3 container instances.
- -p 8080:80: map port 8080 on host → port 80 inside containers.
