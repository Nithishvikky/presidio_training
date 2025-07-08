docker service create \
  --name=visualizer \
  --publish=8080:8080 \
  --constraint=node.role==manager \
  --mount=type=bind,src=/var/run/docker.sock,dst=/var/run/docker.sock \
  --network viz-network \
  dockersamples/visualizer

- --name=visualizer → service name
- --publish=8080:8080 → maps container port 8080 to host port 8080
- --constraint=node.role==manager → only runs on the Swarm manager node
- --mount=... → allows visualizer to access Docker info
- --network viz-network → attach to created overlay network
- dockersamples/visualizer → the official visualizer image


http://localhost:8080 by visting this port,

- Will see a visual map of,

    - Swarm nodes
    - Running containers
    - Their placement across the cluster