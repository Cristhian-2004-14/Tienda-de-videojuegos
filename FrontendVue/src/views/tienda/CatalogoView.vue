<script setup>
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import PaginacionRegistros from '../../components/common/PaginacionRegistros.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';
import { useCarritoStore } from '../../stores/carrito';
import { useTiendaUiStore } from '../../stores/tiendaUi';

const datosStore = useDatosApiStore();
const carritoStore = useCarritoStore();
const uiStore = useTiendaUiStore();
const { productos } = storeToRefs(datosStore);
const { busqueda, categoria } = storeToRefs(uiStore);

const bannersHero = [
  {
    id: 1,
    tag: 'X-STORE KINETIC · CONSOLAS',
    titulo: 'Xbox & PS5 Slim',
    descripcion: 'Rendimiento de nueva generación a 4K y 120 FPS. Garantía oficial y entrega inmediata.',
    badge: 'CONSOLAS DE NUEVA GENERACIÓN',
    categoriaFiltro: 'Consolas',
    enlaceTexto: 'Explorar Consolas',
    imagenProducto: { id: 1, nombre: 'Xbox Series X', categoria: 'Consolas', marca: 'Microsoft', imagenUrl: '/hero/consolas.jpg' }
  },
  {
    id: 2,
    tag: 'CATÁLOGO DE VIDEOJUEGOS',
    titulo: 'Grandes Lanzamientos',
    descripcion: 'Explora la colección de títulos para Xbox, PlayStation y Nintendo Switch al mejor precio.',
    badge: 'JUEGOS MÁS VENDIDOS',
    categoriaFiltro: 'Videojuegos',
    enlaceTexto: 'Ver Catálogo de Juegos',
    imagenProducto: { id: 3, nombre: 'Elden Ring', categoria: 'Videojuegos', marca: 'Bandai Namco', imagenUrl: '/hero/juegos.jpg' }
  },
  {
    id: 3,
    tag: 'ACCESORIOS KINETIC PRO',
    titulo: 'Accesorios Pro',
    descripcion: 'Eleva tu nivel con mandos inalámbricos, headsets con sonido envolvente y almacenamiento ultra rápido.',
    badge: 'HARDWARE & PERIFÉRICOS',
    categoriaFiltro: 'Accesorios',
    enlaceTexto: 'Ver Accesorios Pro',
    imagenProducto: { id: 2, nombre: 'DualSense Controller', categoria: 'Accesorios', marca: 'Sony', imagenUrl: '/hero/accesorios.jpg' }
  },
  {
    id: 4,
    tag: 'SERVICIO TÉCNICO ESPECIALIZADO',
    titulo: 'Servicio Técnico',
    descripcion: 'Limpieza preventiva, cambio de pasta térmica, reparación de puertos y controles en tiempo récord.',
    badge: 'SOPORTE TÉCNICO OFICIAL',
    esServicio: true,
    enlaceRuta: '/tienda/servicio/1',
    enlaceTexto: 'Consultar Servicio Técnico',
    imagenProducto: { id: 5, nombre: 'Servicio Técnico', categoria: 'Accesorios', marca: 'X-Store Tech', imagenUrl: '/hero/servicio.jpg' }
  }
];

const indiceActivo = ref(0);
const pagina = ref(1);
const porPagina = 9;
let temporizador;

const bannerActivo = computed(() => bannersHero[indiceActivo.value] || bannersHero[0]);

const estaSinStock = (producto) => {
  if (!producto || producto.stock <= 0) return true;
  const item = carritoStore.items.find((i) => i.productoId === producto.id);
  return (item?.cantidad || 0) >= producto.stock;
};

const productosFiltrados = computed(() => {
  const termino = busqueda.value.trim().toLowerCase();
  return productos.value
    .filter((producto) => categoria.value === 'Todo' || producto.categoria === categoria.value)
    .filter((producto) => !termino || [producto.nombre, producto.marca, producto.categoria]
      .some((valor) => valor.toLowerCase().includes(termino)));
});

const productosPagina = computed(() => productosFiltrados.value.slice(
  (pagina.value - 1) * porPagina,
  pagina.value * porPagina,
));

function cambiarDiapositiva(direccion) {
  indiceActivo.value = (indiceActivo.value + direccion + bannersHero.length) % bannersHero.length;
}

function irAProductos() {
  document.querySelector('#productos')?.scrollIntoView({ behavior: 'smooth' });
}

watch([busqueda, categoria], () => { pagina.value = 1; });

onMounted(async () => {
  await datosStore.cargarRecurso('productos', productosApi);
  temporizador = window.setInterval(() => cambiarDiapositiva(1), 5500);
});

onBeforeUnmount(() => window.clearInterval(temporizador));
</script>

<template>
  <TiendaLayout>
    <section class="hero">
      <div class="hero-bg-art">
        <img :src="bannerActivo.imagenProducto.imagenUrl" :alt="bannerActivo.titulo" class="hero-bg-img" />
      </div>

      <div class="hero-copy">
        <span class="badge-hero">{{ bannerActivo.badge }}</span>
        <p class="tag">{{ bannerActivo.tag }}</p>
        <h1>{{ bannerActivo.titulo }}</h1>
        <p>{{ bannerActivo.descripcion }}</p>
        <div class="hero-acciones">
          <button v-if="!bannerActivo.esServicio" class="btn-hero" @click="uiStore.seleccionarCategoria(bannerActivo.categoriaFiltro); irAProductos()">
            {{ bannerActivo.enlaceTexto }} <span>→</span>
          </button>
          <router-link v-else :to="bannerActivo.enlaceRuta" class="btn-hero">
            {{ bannerActivo.enlaceTexto }} <span>→</span>
          </router-link>
        </div>
      </div>
      <div class="controles">
        <button aria-label="Banner anterior" @click="cambiarDiapositiva(-1)">←</button>
        <div>
          <button v-for="(_, indice) in bannersHero" :key="indice" :class="{ activo: indice === indiceActivo }" :aria-label="`Mostrar banner ${indice + 1}`" @click="indiceActivo = indice"></button>
        </div>
        <button aria-label="Banner siguiente" @click="cambiarDiapositiva(1)">→</button>
      </div>
      <div class="hero-index mono">{{ String(indiceActivo + 1).padStart(2,'0') }} / {{ String(bannersHero.length).padStart(2,'0') }}</div>
    </section>
    <main id="productos" class="catalogo">
      <div class="titulo"><div><p class="sobre">EXPLORA EL CATÁLOGO</p><h2>{{ busqueda ? `Resultados para “${busqueda}”` : 'Encuentra tu próxima aventura' }}</h2></div><p>{{ productosFiltrados.length }} productos</p></div>
      <div class="filtros"><button v-for="filtro in ['Todo','Consolas','Videojuegos','Accesorios']" :key="filtro" :class="{ activo: categoria === filtro }" @click="uiStore.seleccionarCategoria(filtro)">{{ filtro }}</button><span></span><select aria-label="Ordenar"><option>Más destacados</option><option>Precio: menor a mayor</option></select></div>
      <div class="grid">
        <router-link v-for="producto in productosPagina" :key="producto.id" :to="`/tienda/producto/${producto.id}`" class="card">
          <div class="media-card"><span v-if="producto.stock < 5 && producto.stock > 0" class="alerta">ÚLTIMAS UNIDADES</span><ProductoVisual :producto="producto" /></div>
          <div class="info"><p>{{ producto.categoria }} · {{ producto.marca }}</p><h3>{{ producto.nombre }}</h3><div><strong>${{ producto.precioVenta.toFixed(2) }}</strong><button aria-label="Agregar a la selección" :disabled="estaSinStock(producto)" :title="estaSinStock(producto) ? 'Stock máximo alcanzado' : 'Agregar a la selección'" @click.prevent="carritoStore.agregarProducto(producto)"><span class="material-symbols-outlined">add_shopping_cart</span></button></div></div>
        </router-link>
      </div>
      <PaginacionRegistros v-model:pagina="pagina" :total="productosFiltrados.length" :por-pagina="porPagina" />
      <div v-if="!productosFiltrados.length" class="sin-resultados"><span class="material-symbols-outlined">search_off</span><h3>No encontramos productos</h3><p>Prueba otra búsqueda o cambia la categoría.</p><button @click="busqueda = ''; uiStore.seleccionarCategoria('Todo')">Mostrar todo</button></div>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.hero{min-height:540px;position:relative;overflow:hidden;display:flex;align-items:center;padding:55px clamp(25px,7vw,110px);background-color:#101010}.hero::before{content:"";position:absolute;inset:0;background:radial-gradient(circle at 75% 50%,rgba(121,221,104,.16) 0%,transparent 65%);pointer-events:none;z-index:2}.hero-bg-art{position:absolute;top:0;bottom:0;right:0;width:66%;z-index:1;pointer-events:none;overflow:hidden;mask-image:linear-gradient(to right,transparent 0%,rgba(0,0,0,.5) 20%,black 45%);-webkit-mask-image:linear-gradient(to right,transparent 0%,rgba(0,0,0,.5) 20%,black 45%)}.hero-bg-img{width:100%;height:100%;object-fit:cover;object-position:center right;display:block;transition:opacity .5s ease,transform .5s ease}.hero-copy{z-index:3;max-width:620px}.badge-hero{display:inline-block;background:rgba(121,221,104,.15);border:1px solid #79dd68;color:#79dd68;padding:5px 12px;border-radius:4px;font-size:10px;font-weight:900;letter-spacing:.1em;text-transform:uppercase;margin-bottom:14px}.tag,.sobre{color:#79dd68;font-size:11px;font-weight:900;letter-spacing:.16em}.hero h1{font-size:clamp(46px,7vw,82px);line-height:.94;letter-spacing:-.06em;margin:10px 0 22px}.hero-copy>p:not(.tag){color:#c7cec4;line-height:1.65;max-width:520px}.hero-acciones{display:flex;gap:12px;flex-wrap:wrap;margin-top:27px}.hero-acciones a,.hero-acciones button,.hero-acciones .btn-hero{min-height:48px;display:inline-flex;gap:20px;align-items:center;border:0;border-radius:5px;padding:14px 20px;font-weight:800;cursor:pointer;text-decoration:none}.hero-acciones a,.hero-acciones .btn-hero{background:#107c10;color:#fff}.hero-acciones button{background:#252525;color:#fff}.hero-index{position:absolute;bottom:28px;right:7vw;color:#8b9788;font-size:11px;z-index:3}.controles{position:absolute;z-index:3;bottom:24px;left:clamp(25px,7vw,110px);display:flex;align-items:center;gap:13px}.controles>button{width:36px;height:36px;border:1px solid #ffffff28;background:#171717;color:#fff;cursor:pointer}.controles>div{display:flex;gap:6px}.controles>div button{width:24px;height:4px;padding:0;border:0;background:#ffffff35;cursor:pointer}.controles>div button.activo{background:#79dd68}.hero-vacio{grid-template-columns:1fr}.catalogo{max-width:1440px;margin:auto;padding:68px clamp(20px,4vw,54px)}.titulo{display:flex;justify-content:space-between;align-items:end}.titulo h2{font-size:clamp(27px,4vw,40px);margin-top:10px}.titulo>p{color:#8d958a;font-size:12px}.filtros{display:flex;gap:10px;margin:30px 0;align-items:center;border-bottom:1px solid #292929;padding-bottom:18px}.filtros button,.filtros select{border:0;background:transparent;color:#a2a2a2;padding:10px 14px}.filtros button.activo{background:#107c10;color:#fff;border-radius:5px}.filtros span{flex:1}.filtros select{background:#1b1b1b;border-radius:5px}.grid{display:grid;grid-template-columns:repeat(3,1fr);gap:20px}.card{min-height:390px;position:relative;background:#191919;border:1px solid #ffffff10;border-radius:12px;overflow:hidden;isolation:isolate;transition:transform .3s ease,border-color .3s ease,box-shadow .3s ease}.card:hover{transform:translateY(-6px);border-color:#79dd6866;box-shadow:0 22px 55px #0009}.media-card{position:absolute;inset:0;background:#222}.media-card:after{content:"";position:absolute;inset:0;z-index:2;background:linear-gradient(180deg,transparent 35%,rgba(7,9,7,.18) 53%,rgba(7,9,7,.97) 84%);pointer-events:none}.media-card :deep(.visual){width:100%;height:100%;min-height:0}.alerta{position:absolute;z-index:4;top:14px;left:14px;background:#79dd68;color:#053706;padding:6px 9px;border-radius:4px;font-size:9px;font-weight:900}.info{position:absolute;z-index:3;inset:auto 0 0;padding:24px}.info>p{font-size:10px;color:#9ff18f;text-transform:uppercase}.info h3{font-size:21px;line-height:1.15;margin:8px 0 18px;text-shadow:0 2px 12px #000}.info>div{display:flex;align-items:center;justify-content:space-between}.info strong{font-size:21px}.info button{display:grid;place-items:center;width:42px;height:42px;border:1px solid #ffffff26;border-radius:50%;background:#107c10;color:#fff;cursor:pointer}.info button:hover{background:#79dd68;color:#092509}
@media(max-width:850px){.hero{min-height:480px;padding:40px 25px}.hero-bg-art{width:100%;opacity:.4}.grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:560px){.grid{grid-template-columns:1fr}.titulo{align-items:start;gap:10px;flex-direction:column}.filtros{overflow:auto}.filtros span{display:none}}
.sin-resultados{text-align:center;padding:70px 20px;background:#191919;border:1px solid #2d2d2d;border-radius:8px}.sin-resultados>span{font-size:45px;color:#79dd68}.sin-resultados h3{font-size:22px;margin:12px 0 7px}.sin-resultados p{color:#8d958a;margin-bottom:20px}.sin-resultados button{border:0;background:#107c10;color:#fff;padding:12px 18px;border-radius:5px;font-weight:800}
</style>
