<script setup>
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';

// Consume el store Singleton de sesión (auth.js) para validar contra los
// usuarios mock y redirigir — sin autenticación real (ver sección 9 de CLAUDE.md).
const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const username = ref('');
const password = ref('');
const cargando = ref(false);
const error = ref('');

async function manejarSubmit() {
  error.value = '';
  cargando.value = true;

  const exito = await authStore.iniciarSesion(username.value, password.value);
  cargando.value = false;

  if (exito) {
    router.push(typeof route.query.redirect === 'string' ? route.query.redirect : '/admin');
  } else {
    error.value = 'Usuario o contraseña incorrectos o BackendApi no está disponible.';
  }
}
</script>

<template>
  <main class="login-page">
    <div class="brand">
      <div class="brand-badge">KC</div>
    </div>

    <div class="login-card">
      <header class="login-header">
        <h1>Bienvenido de nuevo</h1>
        <p>Ingresa tus credenciales para acceder a Kinetic Console</p>
      </header>

      <form class="login-form" @submit.prevent="manejarSubmit">
        <div class="field">
          <label for="username">Username</label>
          <div class="input-wrap">
            <span class="material-symbols-outlined">person</span>
            <input
              id="username"
              v-model="username"
              type="text"
              placeholder="nombre.usuario"
              required
            />
          </div>
        </div>

        <div class="field">
          <label for="password">Password</label>
          <div class="input-wrap">
            <span class="material-symbols-outlined">lock</span>
            <input
              id="password"
              v-model="password"
              type="password"
              placeholder="••••••••"
              required
            />
          </div>
        </div>

        <p v-if="error" class="error-msg">{{ error }}</p>

        <div class="actions">
          <button type="submit" class="btn-primary" :disabled="cargando">
            <span v-if="!cargando">Iniciar sesión</span>
            <span v-else>Autenticando...</span>
            <span class="material-symbols-outlined">login</span>
          </button>
          <a href="#" class="forgot-link">Olvidé mi contraseña</a>
        </div>
      </form>
    </div>

    <footer class="login-footer mono">KINETIC CONSOLE ADMIN v1.0.0</footer>
  </main>
</template>

<style scoped>
.login-page {
  min-height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--space-md);
  gap: var(--space-lg);
}

.brand-badge {
  width: 72px;
  height: 72px;
  border-radius: var(--radius-lg);
  background: var(--color-primary-container);
  color: #fff;
  font-weight: 800;
  font-size: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-card {
  width: 100%;
  max-width: 400px;
  background: var(--color-surface-container-low);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: var(--radius-md);
  padding: var(--space-lg);
}

.login-header {
  text-align: center;
  margin-bottom: var(--space-md);
  display: flex;
  flex-direction: column;
  gap: var(--space-xs);
}

.login-header h1 {
  font-size: 20px;
  font-weight: 600;
}

.login-header p {
  color: var(--color-on-surface-variant);
  font-size: 14px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: var(--space-md);
}

.field {
  display: flex;
  flex-direction: column;
  gap: var(--space-xs);
}

.field label {
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  color: var(--color-on-surface-variant);
  padding: 0 var(--space-xs);
}

.input-wrap {
  position: relative;
  display: flex;
  align-items: center;
}

.input-wrap .material-symbols-outlined {
  position: absolute;
  left: var(--space-sm);
  color: var(--color-on-surface-variant);
  font-size: 20px;
}

.input-wrap input {
  width: 100%;
  padding: var(--space-sm) var(--space-md) var(--space-sm) 40px;
  background: var(--color-surface-container-lowest);
  border: 1px solid var(--color-surface-container-high);
  border-radius: var(--radius);
  color: var(--color-on-surface);
  outline: none;
  transition: border-color 0.2s;
}

.input-wrap input::placeholder {
  color: rgba(190, 202, 183, 0.4);
}

.input-wrap input:focus {
  border-color: var(--color-primary-container);
}

.error-msg {
  color: var(--color-error);
  font-size: 13px;
}

.actions {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-md);
  padding-top: var(--space-sm);
}

.btn-primary {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-sm);
  background: var(--color-primary-container);
  color: #fff;
  font-weight: 700;
  border: none;
  border-radius: var(--radius);
  padding: var(--space-sm) 0;
  transition: opacity 0.2s, transform 0.1s;
}

.btn-primary:hover:not(:disabled) {
  opacity: 0.9;
}

.btn-primary:active:not(:disabled) {
  transform: scale(0.98);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.forgot-link {
  font-size: 14px;
  color: var(--color-on-surface-variant);
  transition: color 0.2s;
}

.forgot-link:hover {
  color: var(--color-primary);
  text-decoration: underline;
}

.login-footer {
  opacity: 0.4;
  font-size: 12px;
  color: var(--color-on-surface-variant);
}
</style>
