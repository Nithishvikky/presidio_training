FROM node:22-alpine

WORKDIR /app

COPY . .

RUN npm install express

EXPOSE 5015

CMD ["node", "server.js"]
