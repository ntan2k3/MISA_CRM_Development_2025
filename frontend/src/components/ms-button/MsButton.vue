<script setup>
//#region Props
defineProps({
  icon: {
    type: Object,
    default: null,
  },
  positionIcon: {
    type: String,
    default: "left",
    validator: (value) => ["left", "right"].includes(value),
  },
  type: {
    type: String,
    default: "default",
    validator: (value) => ["default", "primary", "secondary", "waring", "danger"].includes(value),
  },
  variant: {
    type: String,
    default: "solid",
    validator: (value) => ["solid", "outlined", "dashed"].includes(value),
  },
});
//#endregion
</script>

<template>
  <button :class="['ms-button', `ms-button--${type}`, `ms-button--${variant}`]">
    <div
      v-if="icon && positionIcon === 'left'"
      class="ms-button__icon ms-button__icon--left"
      :class="icon"
    ></div>

    <div class="ms-button__content">
      <slot></slot>
    </div>

    <div
      v-if="icon && positionIcon === 'right'"
      class="ms-button__icon ms-button__icon--right"
      :class="icon"
    ></div>
  </button>
</template>

<style scoped>
.ms-button {
  height: 32px;
  font-size: 13px;
  font-weight: 500;
  padding: 5px 8px;
  border-radius: 4px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 1px solid #d3d7de;
  background-color: white;
  cursor: pointer;
}

/*====== Type + Variant ======*/

/* Default */

.ms-button--default.ms-button--solid {
  border: 1px solid white;
  color: black;
  background-color: white;
}

.ms-button--default.ms-button--solid:hover {
  background-color: #f0f2f4;
}

.ms-button--default.ms-button--outlined {
  border: 1px solid black;
  color: black;
  background-color: transparent;
}

.ms-button--default.ms-button--outlined:hover {
  background-color: #f0f2f4;
}

.ms-button--default.ms-button--dashed {
  border: 1px dashed white;
  color: white;
  background-color: black;
}

/* Primary */

.ms-button--primary.ms-button--solid {
  border: 1px solid #4262f0;
  color: white;
  background-color: #4262f0;
}

.ms-button--primary.ms-button--solid:hover {
  background-color: #2b4eee;
}

.ms-button--primary.ms-button--outlined {
  border: 1px solid #4262f0;
  color: #4262f0;
  background-color: white;
}

.ms-button--primary.ms-button--outlined:hover {
  background-color: #e7ebfd;
}

.ms-button--primary.ms-button--dashed {
  border: 1px dashed #4262f0;
  color: #4262f0;
  background-color: white;
}

/* Secondary */

.ms-button--secondary.ms-button--solid {
  border: 1px solid #d3d7de;
  color: white;
  background-color: #d3d7de;
}

.ms-button--secondary.ms-button--outlined {
  border: 1px solid #d3d7de;
  color: #d3d7de;
  background-color: white;
}

.ms-button--secondary.ms-button--outlined:hover {
  background-color: #f0f2f4;
}

.ms-button--secondary.ms-button--dashed {
  border: 1px dashed #d3d7de;
  color: #d3d7de;
  background-color: white;
}

/* Warning */

.ms-button--warning.ms-button--solid {
  border: 1px solid #ffab00;
  color: white;
  background-color: #ffab00;
}

.ms-button--warning.ms-button--solid:hover {
  background-color: #e89a00;
}

.ms-button--warning.ms-button--outlined {
  border: 1px solid #ffab00;
  color: #ffab00;
  background-color: white;
}

.ms-button--warning.ms-button--outlined:hover {
  background-color: #fff4da;
}

.ms-button--warning.ms-button--dashed {
  border: 1px dashed #ffab00;
  color: #ffab00;
  background-color: white;
}

/* Danger */

.ms-button--danger.ms-button--solid {
  border: 1px solid #e53935;
  color: white;
  background-color: #e53935;
}

.ms-button--danger.ms-button--solid:hover {
  background-color: #d32f2f;
}

.ms-button--danger.ms-button--outlined {
  border: 1px solid #e53935;
  color: #e53935;
  background-color: white;
}

.ms-button--danger.ms-button--outlined:hover {
  background-color: #ffe7e6;
}

.ms-button--danger.ms-button--dashed {
  border: 1px dashed #e53935;
  color: #e53935;
  background-color: white;
}

.ms-button__content {
  white-space: nowrap;
}

.ms-button__icon {
  display: inline-flex;
  align-items: center;
}

.ms-button__icon--left {
  margin-right: 8px;
}

.ms-button__icon--right {
  margin-left: 8px;
}
</style>
