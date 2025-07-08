docker service create \
  --name webapp \
  --replicas 3 \
  --update-delay 10s \
  httpd

- --name webapp → Name of the service.
- --replicas 3 → Starts 3 containers.
- --update-delay 10s → Wait 10 seconds before updating the next replica during updates.
- httpd → The base image (Apache server).

docker service ls

- Make sure 3/3 replicas are running

#### Update the image to httpd:alpine

docker service update \
  --image httpd:alpine \
  webapp


- Now Docker Swarm will,
    - Stop one container.
    - Replace it with the new version.
    - Wait 10 seconds, then move to the next replica.


watch docker service ps webapp

- To watch the rolling updates