const express = require("express");
require("dotenv").config();
const renderApi = require("@api/render-api");

const app = express();
const PORT = process.env.PORT || 3000;
const API_KEY = process.env.RENDER_API_KEY;

if (!API_KEY) {
  console.error("Missing RENDER_API_KEY in environment variables");
  process.exit(1);
}

renderApi.auth(API_KEY);

app.get("/", async (req, res) => {
  try {
    const { data } = await renderApi.listServices({ includePreviews: "true", limit: "20" });
    res.json(data);
  } catch (error) {
    console.error("Error fetching services:", error);
    res.status(500).json({ error: "Failed to fetch services" });
  }
});

app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
