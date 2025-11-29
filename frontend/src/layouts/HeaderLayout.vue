<script setup>
import { RouterLink, useRoute } from "vue-router";
import { computed } from "vue";

const menu = [
  { icon: "icon-dashboard", text: "Bàn làm việc", to: "/dashboard" },
  { icon: "icon-customer", text: "Khách hàng", to: "/" },
  { icon: "icon-opportunity", text: "Cơ hội", to: "/opportunities" },
  { icon: "icon-quote", text: "Báo giá", to: "/quotes" },
  { icon: "icon-sale-order", text: "Đơn hàng", to: "/orders" },
  { icon: "icon-product", text: "Hàng hóa", to: "/products" },
  { icon: "icon-report", text: "Báo cáo", to: "/reports" },
];

const route = useRoute();
const menuWithActive = computed(() =>
  menu.map((item) => ({
    ...item,
    active: route.path === item.to,
  }))
);
</script>

<template>
  <div class="header flex-row justify-between">
    <!-- Left -->
    <div class="header-left flex-center">
      <!-- Logo -->
      <div class="header-logo flex items-center" />

      <!-- Menu -->
      <div class="header-menu flex-center">
        <router-link
          v-for="item in menuWithActive"
          :key="item.to"
          :to="item.to"
          class="header-item flex-center"
          :class="{ active: item.active }"
        >
          <div :class="['icon-mask', item.icon, 'icon-16', 'mb-2', { active: item.active }]" />
          <div class="header-item-text" :class="{ active: item.active }">{{ item.text }}</div>
          <div v-if="item.active" class="active-row"></div>
        </router-link>

        <div class="header-item flex-center">
          <div class="icon-mask icon-other icon-16" />
          <div class="header-item-text">Tất cả</div>
        </div>
      </div>
    </div>

    <!-- Right -->
    <div class="header-right flex-center">
      <!-- Avatar -->
      <div class="avatar" />
    </div>
  </div>
</template>

<style scoped>
.header {
  position: relative;
  height: 48px;
  padding-right: 14px;
  color: #1e2633;
  background-color: #fff;
  box-shadow: 0 2px 4px #1f222929;
  z-index: 100;
  overflow: hidden;
}

.header-left,
.header-right {
  height: 48px;
}

.header-left {
  padding-left: 52px;
}

.header-logo {
  width: 32px;
  height: 32px;
  gap: 12px;
  cursor: pointer;
}

.header-logo::before {
  content: "";
  display: inline-block;
  min-width: 32px;
  height: 32px;
  background-image: url("@/assets/logos/Logo_CRM_App_New.svg");
  background-size: cover;
  background-repeat: no-repeat;
}

.header-logo::after {
  content: "CRM";
  font-weight: 700;
  font-size: 18px;
  color: #111827;
}

.header-menu {
  height: 48px;
  margin-right: 16px;
  margin-left: 68px;
}

.header-item {
  position: relative;
  border-radius: 4px;
  padding: 0 8px;
  height: 32px;
  font-size: 13px;
  color: #1f2229;
  text-decoration: none;
  transition: 0.3s all ease-in-out;
}

.header-item:hover {
  background-color: #f0f2f4;
  cursor: pointer;
}

.header-item.active:hover {
  background-color: transparent;
}

.header-item:last-child {
  padding: 12px;
}

.icon-mask.active {
  background-color: #4262f0;
}

.header-item-text.active {
  color: #4262f0;
  font-weight: 500;
}

.active-row {
  position: absolute;
  background-color: #4262f0;
  height: 2px;
  bottom: -8px;
  left: 8px;
  right: 8px;
}

.header-item-text {
  margin-left: 8px;
}

.header-right {
  padding: 0 6px;
}

.avatar {
  background-image: url("@/assets/images/avatar.png");
  background-repeat: no-repeat;
  background-size: cover;
  width: 32px;
  height: 32px;
  border-radius: 50%;
}
</style>
