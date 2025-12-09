import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      redirect: "/customers",
    },
    // Customers
    {
      path: "/customers",
      name: "customers",
      component: () => import("@/views/customer/CustomerView.vue"),
    },
    {
      path: "/customers/add",
      name: "customer-add",
      component: () => import("@/views/customer/CustomerFormView.vue"),
    },
    {
      path: "/customers/:id",
      name: "customer-edit",
      component: () => import("@/views/customer/CustomerFormView.vue"),
    },
  ],
});

export default router;
