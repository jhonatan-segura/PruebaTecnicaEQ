import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7232", // ✅ tu URL base
  headers: {
    "Content-Type": "application/json"
  }
});

export default api;
