const express = require('express');
const app = express();
const port = 5015;

app.get('/api', (req, res) => {
  res.json({ message: 'Hello from backend!' });
});

app.listen(port, () => {
  console.log(`Backend API running on port ${port}`);
});
