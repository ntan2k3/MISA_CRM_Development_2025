<script setup>
import HeaderLayout from "@/layouts/HeaderLayout.vue";
import ContentLayout from "@/layouts/ContentLayout.vue";
</script>

<template>
  <div class="container flex-col">
    <!-- Header -->
    <header-layout />
    <!-- Content -->
    <content-layout />
  </div>
</template>

<style scoped>
.container {
  width: 100%;
  height: 100vh;
}
</style>
