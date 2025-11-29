import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

import "./assets/css/styles.css";
import "./assets/css/commons.css";
import "./assets/css/icons.css";

const app = createApp(App);

app.use(router);

app.mount("#app");
