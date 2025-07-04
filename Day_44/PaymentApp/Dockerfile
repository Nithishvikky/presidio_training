FROM node:22-alpine AS build

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm install -g @angular/cli

RUN npm install

COPY . .

RUN ng build --configuration=production

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY --from=build /app/dist/PaymentApp/browser /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD [ "nginx", "-g", "daemon off;" ]
