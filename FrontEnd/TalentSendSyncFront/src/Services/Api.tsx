import axios from "axios";
import { getStoredToken } from "../Contexts/useAuth";

const api = axios.create({
    
    //azure https://apipetshoop-f8b7cpataca3fhd8.canadacentral-01.azurewebsites.net/api  
    //http://localhost:5079/api
    baseURL: "/api", //usa apenas api quando tiver deploy no nginx localhost
    
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.data?.detail) {
            error.message = error.response.data.detail;
        } else if (error.response?.data?.message) {
            error.message = error.response.data.message;
        } else if (error.response?.data) {
            error.message = JSON.stringify(error.response.data);
        }
        return Promise.reject(error);
    }
);

api.interceptors.request.use((config) => {
    const token = getStoredToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default api;