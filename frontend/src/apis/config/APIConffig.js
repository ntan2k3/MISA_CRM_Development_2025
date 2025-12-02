import axios from "axios";

const baseURL = "https://localhost:7181";

let api = axios.create({
  baseURL: baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

export default api;
