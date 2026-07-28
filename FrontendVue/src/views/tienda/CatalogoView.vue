<script setup>
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';
import { useCarritoStore } from '../../stores/carrito';
import { useTiendaUiStore } from '../../stores/tiendaUi';

const datosStore = useDatosApiStore();
const carritoStore = useCarritoStore();
const uiStore = useTiendaUiStore();
const { productos } = storeToRefs(datosStore);
const { busqueda, categoria } = storeToRefs(uiStore);
const destacados = ref([]);
const indiceActivo = ref(0);
let temporizador;

const productoActivo = computed(() => destacados.value[indiceActivo.value] || productos.value[0]);
const productosFiltrados = computed(() => {
  const termino = busqueda.value.trim().toLowerCase();
  return productos.value
    .filter((producto) => categoria.value === 'Todo' || producto.categoria === categoria.value)
    .filter((producto) => !termino || [producto.nombre, producto.marca, producto.categoria]
      .some((valor) => valor.toLowerCase().includes(termino)));
});

function prepararDestacados() {
  destacados.value = [...productos.value]
    .filter((producto) => producto.stock > 0)
    .sort(() => Math.random() - 0.5)
    .slice(0, 5);
  indiceActivo.value = 0;
}

function cambiarDiapositiva(direccion) {
  if (!destacados.value.length) return;
  indiceActivo.value = (indiceActivo.value + direccion + destacados.value.length) % destacados.value.length;
}

watch(productos, prepararDestacados, { immediate: true });
onMounted(async () => {
  await datosStore.cargarRecurso('productos', productosApi);
  temporizador = window.setInterval(() => cambiarDiapositiva(1), 5500);
});
onBeforeUnmount(() => window.clearInterval(temporizador));
</script>

<template>
  <TiendaLayout>
    <section v-if="productoActivo" class="hero">
      <div class="hero-copy">
        <p class="tag">DESTACADO DEL CATÁLOGO · {{ productoActivo.categoria }}</p>
        <h1>{{ productoActivo.nombre }}</h1>
        <p>{{ productoActivo.marca }} · {{ productoActivo.stock }} unidades disponibles. Agrégalo a tu selección y consulta tu pedido por WhatsApp.</p>
        <div class="hero-acciones">
          <router-link :to="`/tienda/producto/${productoActivo.id}`">Ver producto <span>→</span></router-link>
          <button @click="carritoStore.agregarProducto(productoActivo)"><span class="material-symbols-outlined">add_shopping_cart</span>Agregar</button>
        </div>
      </div>
      <div class="hero-product"><div class="halo"></div><ProductoVisual :producto="productoActivo" /><strong>${{ productoActivo.precioVenta.toFixed(2) }}</strong></div>
      <div class="controles">
        <button aria-label="Producto anterior" @click="cambiarDiapositiva(-1)">←</button>
        <div><button v-for="(_, indice) in destacados" :key="indice" :class="{ activo: indice === indiceActivo }" :aria-label="`Mostrar producto ${indice + 1}`" @click="indiceActivo = indice"></button></div>
        <button aria-label="Producto siguiente" @click="cambiarDiapositiva(1)">→</button>
      </div>
      <div class="hero-index mono">{{ String(indiceActivo + 1).padStart(2,'0') }} / {{ String(destacados.length).padStart(2,'0') }}</div>
    </section>
    <section v-else class="hero hero-vacio"><div class="hero-copy"><p class="tag">X-STORE</p><h1>Tu próxima aventura empieza aquí</h1><p>Estamos preparando el catálogo. Vuelve a intentarlo en unos instantes.</p></div></section>
    <main id="productos" class="catalogo">
      <div class="titulo"><div><p class="sobre">EXPLORA EL CATÁLOGO</p><h2>{{ busqueda ? `Resultados para “${busqueda}”` : 'Encuentra tu próxima aventura' }}</h2></div><p>{{ productosFiltrados.length }} productos</p></div>
      <div class="filtros"><button v-for="filtro in ['Todo','Consolas','Videojuegos','Accesorios']" :key="filtro" :class="{ activo: categoria === filtro }" @click="uiStore.seleccionarCategoria(filtro)">{{ filtro }}</button><span></span><select aria-label="Ordenar"><option>Más destacados</option><option>Precio: menor a mayor</option></select></div>
      <div class="grid">
        <router-link v-for="producto in productosFiltrados" :key="producto.id" :to="`/tienda/producto/${producto.id}`" class="card">
          <div class="visual"><span v-if="producto.stock < 5 && producto.stock > 0" class="alerta">ÚLTIMAS UNIDADES</span><ProductoVisual :producto="producto" /></div>
          <div class="info"><p>{{ producto.categoria }} · {{ producto.marca }}</p><h3>{{ producto.nombre }}</h3><div><strong>${{ producto.precioVenta.toFixed(2) }}</strong><button aria-label="Agregar a la selección" :disabled="producto.stock <= 0" @click.prevent="carritoStore.agregarProducto(producto)"><span class="material-symbols-outlined">add_shopping_cart</span></button></div></div>
        </router-link>
      </div>
      <div v-if="!productosFiltrados.length" class="sin-resultados"><span class="material-symbols-outlined">search_off</span><h3>No encontramos productos</h3><p>Prueba otra búsqueda o cambia la categoría.</p><button @click="busqueda = ''; uiStore.seleccionarCategoria('Todo')">Mostrar todo</button></div>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.hero{min-height:560px;position:relative;overflow:hidden;display:grid;grid-template-columns:1.08fr .92fr;align-items:center;padding:55px clamp(25px,7vw,110px);background:radial-gradient(circle at 78% 40%,#1d531f 0,#122b14 20%,#101010 58%)}.hero:after{content:"";position:absolute;inset:0;background:repeating-linear-gradient(90deg,transparent 0,transparent calc(8.33% - 1px),#ffffff08 calc(8.33% - 1px),#ffffff08 8.33%);pointer-events:none}.hero-copy{z-index:2;max-width:650px}.tag,.sobre{color:#79dd68;font-size:11px;font-weight:900;letter-spacing:.16em}.hero h1{font-size:clamp(46px,7vw,82px);line-height:.94;letter-spacing:-.06em;margin:15px 0 22px}.hero-copy>p:not(.tag){color:#c7cec4;line-height:1.65;max-width:520px}.hero-acciones{display:flex;gap:12px;flex-wrap:wrap;margin-top:27px}.hero-acciones a,.hero-acciones button{min-height:48px;display:inline-flex;gap:20px;align-items:center;border:0;border-radius:5px;padding:14px 20px;font-weight:800}.hero-acciones a{background:#107c10}.hero-acciones button{background:#252525;color:#fff}.hero-product{z-index:1;display:grid;place-items:center;transform:scale(2.15);position:relative}.hero-product strong{position:absolute;transform:scale(.46);bottom:-38px;background:#0d0d0dd9;border:1px solid #79dd68;padding:10px 18px;color:#79dd68;font-size:20px}.halo{position:absolute;width:240px;height:240px;border-radius:50%;background:#79dd6840;filter:blur(35px)}.hero-index{position:absolute;bottom:28px;right:7vw;color:#8b9788;font-size:11px}.controles{position:absolute;z-index:3;bottom:24px;left:clamp(25px,7vw,110px);display:flex;align-items:center;gap:13px}.controles>button{width:36px;height:36px;border:1px solid #ffffff28;background:#171717;color:#fff}.controles>div{display:flex;gap:6px}.controles>div button{width:24px;height:4px;padding:0;border:0;background:#ffffff35}.controles>div button.activo{background:#79dd68}.hero-vacio{grid-template-columns:1fr}.catalogo{max-width:1440px;margin:auto;padding:68px clamp(20px,4vw,54px)}.titulo{display:flex;justify-content:space-between;align-items:end}.titulo h2{font-size:clamp(27px,4vw,40px);margin-top:10px}.titulo>p{color:#8d958a;font-size:12px}.filtros{display:flex;gap:10px;margin:30px 0;align-items:center;border-bottom:1px solid #292929;padding-bottom:18px}.filtros button,.filtros select{border:0;background:transparent;color:#a2a2a2;padding:10px 14px}.filtros button.activo{background:#107c10;color:#fff;border-radius:5px}.filtros span{flex:1}.filtros select{background:#1b1b1b;border-radius:5px}.grid{display:grid;grid-template-columns:repeat(3,1fr);gap:20px}.card{background:#191919;border-radius:8px;overflow:hidden;transition:.2s}.card:hover{transform:translateY(-4px);background:#202020}.visual{height:250px;display:grid;place-items:center;background:#222;position:relative;padding:55px}.alerta{position:absolute;top:13px;left:13px;background:#79dd68;color:#053706;padding:5px 7px;font-size:9px;font-weight:900}.info{padding:19px}.info>p{font-size:10px;color:#79dd68;text-transform:uppercase}.info h3{font-size:18px;margin:7px 0 20px}.info>div{display:flex;align-items:center;justify-content:space-between}.info strong{font-size:19px}.info button{display:grid;place-items:center;width:38px;height:38px;border:0;border-radius:5px;background:#107c10;color:#fff}
@media(max-width:850px){.hero{grid-template-columns:1fr;min-height:570px}.hero-product{opacity:.25;position:absolute;right:10%;transform:scale(2.7)}.grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:560px){.grid{grid-template-columns:1fr}.titulo{align-items:start;gap:10px;flex-direction:column}.filtros{overflow:auto}.filtros span{display:none}}
.sin-resultados{text-align:center;padding:70px 20px;background:#191919;border:1px solid #2d2d2d;border-radius:8px}.sin-resultados>span{font-size:45px;color:#79dd68}.sin-resultados h3{font-size:22px;margin:12px 0 7px}.sin-resultados p{color:#8d958a;margin-bottom:20px}.sin-resultados button{border:0;background:#107c10;color:#fff;padding:12px 18px;border-radius:5px;font-weight:800}
</style>
