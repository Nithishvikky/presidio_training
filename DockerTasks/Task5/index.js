const express = require("express");
const mongoose = require("mongoose");

const app = express();
const PORT = 3000;

const mongoURL = process.env.MONGO_URL || "mongodb://localhost:27017/db";

mongoose.connect(mongoURL)
  .then(() => console.log("Connected to MongoDB"))
  .catch((err) => console.error("MongoDB connection error:", err));

app.get("/", (req, res) => {
  res.send("Node.js + MongoDB running!");
});

app.listen(PORT, () => {
  console.log(`🌐 Server running on http://localhost:${PORT}`);
});
