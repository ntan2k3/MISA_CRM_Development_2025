import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // Dashboard
    {
      path: "/dashboard",
      name: "dashboard",
      component: () => import("@/views/dashboard/DashboardView.vue"),
    },
    // Customers
    {
      path: "/",
      name: "customers",
      component: () => import("@/views/customer/CustomerView.vue"),
    },
  ],
});

export default router;
