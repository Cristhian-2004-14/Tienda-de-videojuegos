<script setup>
import { onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';

const datosStore = useDatosApiStore();
const { productos } = storeToRefs(datosStore);
onMounted(() => datosStore.cargarRecurso('productos', productosApi));
</script>

<template>
  <TiendaLayout>
    <section class="hero">
      <div class="hero-copy"><p class="tag">POTENCIA SIN LÍMITES</p><h1>Kinetic Series X</h1><p>La consola más rápida y potente. Diseñada para una nueva generación de juegos.</p><router-link to="/tienda/producto/1">Comprar ahora <span>→</span></router-link></div>
      <div class="hero-product"><div class="halo"></div><ProductoVisual tipo="Consolas" /></div>
      <div class="hero-index mono">01 / 04</div>
    </section>
    <main id="productos" class="catalogo">
      <div class="titulo"><div><p class="sobre">EXPLORA EL CATÁLOGO</p><h2>Encuentra tu próxima aventura</h2></div><p>{{ productos.length }} productos</p></div>
      <div class="filtros"><button class="activo">Todo</button><button>Consolas</button><button>Videojuegos</button><button>Accesorios</button><span></span><select aria-label="Ordenar"><option>Más destacados</option><option>Precio: menor a mayor</option></select></div>
      <div class="grid">
        <router-link v-for="producto in productos" :key="producto.id" :to="`/tienda/producto/${producto.id}`" class="card">
          <div class="visual"><span v-if="producto.stock < 5 && producto.stock > 0" class="alerta">ÚLTIMAS UNIDADES</span><ProductoVisual :tipo="producto.categoria" /></div>
          <div class="info"><p>{{ producto.categoria }} · {{ producto.marca }}</p><h3>{{ producto.nombre }}</h3><div><strong>${{ producto.precioVenta.toFixed(2) }}</strong><button aria-label="Agregar al carrito"><span class="material-symbols-outlined">add_shopping_cart</span></button></div></div>
        </router-link>
      </div>
    </main>
  </TiendaLayout>
</template>

<style scoped>
.hero{min-height:510px;position:relative;overflow:hidden;display:grid;grid-template-columns:1fr 1fr;align-items:center;padding:55px clamp(25px,7vw,110px);background:linear-gradient(115deg,#0e210f,#101010 62%)}.hero:after{content:"";position:absolute;inset:0;background:repeating-linear-gradient(90deg,transparent 0,transparent calc(8.33% - 1px),#ffffff05 calc(8.33% - 1px),#ffffff05 8.33%);pointer-events:none}.hero-copy{z-index:2;max-width:610px}.tag,.sobre{color:#79dd68;font-size:11px;font-weight:900;letter-spacing:.16em}.hero h1{font-size:clamp(46px,7vw,86px);line-height:.92;letter-spacing:-.065em;margin:15px 0 22px}.hero-copy>p:not(.tag){color:#b8c0b5;line-height:1.65;max-width:480px}.hero-copy a{display:inline-flex;gap:30px;align-items:center;background:#107c10;padding:15px 20px;margin-top:27px;font-weight:800}.hero-product{z-index:1;display:grid;place-items:center;transform:scale(2.4);position:relative}.halo{position:absolute;width:210px;height:210px;border-radius:50%;background:#79dd6820;filter:blur(28px)}.hero-index{position:absolute;bottom:28px;right:7vw;color:#667064;font-size:11px}.catalogo{max-width:1440px;margin:auto;padding:68px clamp(20px,4vw,54px)}.titulo{display:flex;justify-content:space-between;align-items:end}.titulo h2{font-size:clamp(27px,4vw,40px);margin-top:10px}.titulo>p{color:#8d958a;font-size:12px}.filtros{display:flex;gap:10px;margin:30px 0;align-items:center;border-bottom:1px solid #292929;padding-bottom:18px}.filtros button,.filtros select{border:0;background:transparent;color:#a2a2a2;padding:10px 14px}.filtros button.activo{background:#107c10;color:#fff;border-radius:5px}.filtros span{flex:1}.filtros select{background:#1b1b1b;border-radius:5px}.grid{display:grid;grid-template-columns:repeat(3,1fr);gap:20px}.card{background:#191919;border-radius:8px;overflow:hidden;transition:.2s}.card:hover{transform:translateY(-4px);background:#202020}.visual{height:250px;display:grid;place-items:center;background:#222;position:relative;padding:55px}.alerta{position:absolute;top:13px;left:13px;background:#79dd68;color:#053706;padding:5px 7px;font-size:9px;font-weight:900}.info{padding:19px}.info>p{font-size:10px;color:#79dd68;text-transform:uppercase}.info h3{font-size:18px;margin:7px 0 20px}.info>div{display:flex;align-items:center;justify-content:space-between}.info strong{font-size:19px}.info button{display:grid;place-items:center;width:38px;height:38px;border:0;border-radius:5px;background:#107c10;color:#fff}
@media(max-width:850px){.hero{grid-template-columns:1fr;min-height:570px}.hero-product{opacity:.25;position:absolute;right:10%;transform:scale(2.7)}.grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:560px){.grid{grid-template-columns:1fr}.titulo{align-items:start;gap:10px;flex-direction:column}.filtros{overflow:auto}.filtros span{display:none}}
</style>
