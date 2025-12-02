<script setup>
import MsInput from "@/components/ms-input/MsInput.vue";
//#region Props
defineProps({
  leftCol: {
    type: Array,
    default: () => [],
  },
  rightCol: {
    type: Array,
    default: () => [],
  },
});
//#endregion
</script>

<template>
  <div class="ms-form flex-col flex-1">
    <!-- Ảnh -->
    <div class="ms-form__section-image">
      <div class="ms-form__title-image">Ảnh</div>
      <div class="ms-form__avatar"></div>
    </div>

    <!-- Thông tin chung -->
    <div class="ms-form__info">
      <!-- Header -->
      <div class="ms-form__header">Thông tin chung</div>

      <!-- Body -->
      <div class="ms-form__body">
        <div class="ms-form__grid flex">
          <!-- Left -->
          <div class="ms-form__col">
            <div
              v-for="item in leftCol"
              class="ms-form__row flex items-center justify-between"
              :key="item.label"
            >
              <div class="ms-form__label">
                {{ item.label }} <span v-if="item.required" style="color: red">*</span>
              </div>
              <ms-input
                :type="item.type"
                :placeholder="item.placeholder || ''"
                :options="item.options"
              />
            </div>
          </div>

          <!-- Right -->
          <div class="ms-form__col">
            <div
              v-for="item in rightCol"
              class="ms-form__row flex items-center justify-between"
              :key="item.label"
            >
              <div class="ms-form__label">
                {{ item.label }} <span v-if="item.required" style="color: red">*</span>
              </div>
              <ms-input
                :type="item.type"
                :placeholder="item.placeholder || ''"
                :options="item.options"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.ms-form__section-image {
  margin-bottom: 40px;
  padding-left: 56px;
}

.ms-form__title-image {
  font-size: 18px;
  font-weight: 500;
  margin-bottom: 16px;
}

.ms-form__avatar {
  border-radius: 50%;
  width: 48px;
  height: 48px;
  overflow: hidden;
  background: url("@/assets/icons/ICON.svg");
  background-repeat: no-repeat;
  background-position: 0 -848px;
  cursor: pointer;
}

.ms-form__avatar:hover {
  background-position: -48px -848px;
}

.ms-form__header {
  max-width: 1420px;
  margin: 0 56px 4px;
  overflow: hidden;
  font-size: 18px;
  font-weight: 500;
  color: #1f2229;
}

.ms-form__body {
  padding: 8px 56px;
}

.ms-form__grid {
  gap: 80px;
  max-width: 1420px;
}

.ms-form__col {
  flex-basis: 50%;
}

.ms-form__row {
  margin: 4px 0;
  width: 100%;
  padding: 4px 0;
}

.ms-form__label {
  width: 160px;
  font-size: 13px;
  font-weight: 500;
  flex-shrink: 0;
}
</style>
