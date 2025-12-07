import axios from "axios";

const baseURL = `${import.meta.env.VITE_API_BASE_URL}/api/v1`;

let api = axios.create({
  baseURL: baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

export default api;
