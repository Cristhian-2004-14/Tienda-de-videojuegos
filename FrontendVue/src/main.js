import { createApp } from 'vue';
import { createPinia } from 'pinia';
import './style.css';
import App from './App.vue';
import router from './router';
import { inicializarAnalytics } from './firebase';

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.directive('solo-digitos', {
  mounted(elemento) {
    elemento.__soloDigitos = () => {
      const limite = Number(elemento.maxLength) > 0 ? Number(elemento.maxLength) : Infinity;
      const valor = elemento.value.replace(/\D/g, '').slice(0, limite);
      if (valor === elemento.value) return;
      elemento.value = valor;
      elemento.dispatchEvent(new Event('input', { bubbles: true }));
    };
    elemento.addEventListener('input', elemento.__soloDigitos);
  },
  beforeUnmount(elemento) {
    elemento.removeEventListener('input', elemento.__soloDigitos);
  },
});

app.mount('#app');
inicializarAnalytics();
