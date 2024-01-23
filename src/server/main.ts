import express from "express";
import ViteExpress from "vite-express";

const app = express();

app.get("/hello", (_, res) => {
  res.send("Hello Vite + React + TypeScript!");
});

const server = app.listen(3000, () =>
  console.log("Server is listening...")
);

ViteExpress.bind(app, server);