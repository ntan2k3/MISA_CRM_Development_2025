<script setup>
import { RouterView } from "vue-router";
</script>

<template>
  <div class="content flex-col flex-1">
    <router-view />
  </div>
</template>

<style scoped>
.content {
  width: 100%;
  overflow: hidden;
  background: #e2e4e9;
}
</style>
