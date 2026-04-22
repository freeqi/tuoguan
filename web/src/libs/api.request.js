import HttpRequest from "@/libs/axios";
import config from "@/config";
let hostname = window.location.hostname;
let PROD_URL = config.baseURL.pro;

if (hostname == "192.168.10.100") {
  PROD_URL = config.baseURL.dev;
}

const baseUrl =
  process.env.NODE_ENV === "development" ? config.baseURL.dev : PROD_URL;

const axios = new HttpRequest(baseUrl);
export default axios;
