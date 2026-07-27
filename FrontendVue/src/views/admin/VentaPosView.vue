<script setup>
import { computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import AdminLayout from '../../components/AdminLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';

const datosStore = useDatosApiStore();
const { productos } = storeToRefs(datosStore);
const carritoDemo = computed(() => [
  { ...productos.value[0], cantidad: 1 },
  { ...productos.value[1], cantidad: 2 },
].filter((item) => item.id));
onMounted(() => datosStore.cargarRecurso('productos', productosApi));
</script>

<template>
  <AdminLayout titulo="Terminal de venta" buscador-placeholder="Buscar productos o SKU...">
    <template #header>
      <div class="encabezado">
        <div><p class="eyebrow mono">PUNTO DE VENTA / TERMINAL 02</p><h2>Hardware y juegos</h2></div>
        <div class="turno"><span></span>Turno activo</div>
      </div>
    </template>

    <div class="pos-grid">
      <section>
        <div class="filtros">
          <button class="activo">Todos</button><button>Consolas</button><button>Videojuegos</button><button>Accesorios</button>
        </div>
        <div class="productos">
          <article v-for="producto in productos" :key="producto.id" class="producto">
            <div class="visual"><ProductoVisual :tipo="producto.categoria" /></div>
            <div class="producto-info">
              <p class="mono sku">SKU-00{{ producto.id }}</p>
              <h3>{{ producto.nombre }}</h3>
              <p>{{ producto.marca }}</p>
              <div><strong>${{ producto.precioVenta.toFixed(2) }}</strong><span>{{ producto.stock }} disponibles</span></div>
            </div>
          </article>
        </div>
      </section>

      <aside class="orden">
        <div class="orden-titulo"><h3>Orden actual</h3><span class="mono">#V-2048</span></div>
        <label class="cliente"><span class="material-symbols-outlined">person</span><input value="Alex Rivera" aria-label="Cliente" /></label>
        <div class="lineas">
          <article v-for="item in carritoDemo" :key="item.id">
            <div><strong>{{ item.nombre }}</strong><small>${{ item.precioVenta.toFixed(2) }} c/u</small></div>
            <div class="cantidad"><button>−</button><span>{{ item.cantidad }}</span><button>+</button></div>
            <b>${{ (item.precioVenta * item.cantidad).toFixed(2) }}</b>
          </article>
        </div>
        <div class="totales">
          <p><span>Subtotal</span><strong>$639.97</strong></p>
          <p><span>Impuestos</span><strong>$83.20</strong></p>
          <p class="total"><span>Total</span><strong>$723.17</strong></p>
        </div>
        <button class="cobrar"><span class="material-symbols-outlined">point_of_sale</span>Cobrar $723.17</button>
        <button class="cancelar">Cancelar venta</button>
      </aside>
    </div>
  </AdminLayout>
</template>

<style scoped>
.encabezado{display:flex;justify-content:space-between;align-items:end}.encabezado h2{font-size:32px}.eyebrow{font-size:11px;color:#79dd68;margin-bottom:8px}.turno{display:flex;gap:8px;align-items:center;color:#aab4a6;font-size:13px}.turno span{width:8px;height:8px;border-radius:50%;background:#79dd68;box-shadow:0 0 0 4px #79dd6820}
.pos-grid{display:grid;grid-template-columns:minmax(0,1fr) 390px;gap:28px}.filtros{display:flex;gap:8px;margin-bottom:20px}.filtros button{border:1px solid #303030;background:#1b1b1b;color:#bbb;padding:10px 18px;border-radius:6px}.filtros .activo{background:#107c10;border-color:#107c10;color:#fff;font-weight:800}
.productos{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:16px}.producto{display:grid;grid-template-columns:132px 1fr;background:#1b1b1b;border-radius:8px;overflow:hidden;min-height:160px}.visual{display:grid;place-items:center;background:#242424;padding:18px}.producto-info{padding:18px;display:flex;flex-direction:column}.sku{font-size:10px;color:#79dd68}.producto h3{font-size:17px;margin:6px 0}.producto-info>p:not(.sku){color:#999;font-size:12px}.producto-info>div{margin-top:auto;display:flex;justify-content:space-between;align-items:end}.producto-info strong{font-size:19px}.producto-info span{font-size:10px;color:#9eaa9a}
.orden{background:#191919;border-left:3px solid #107c10;padding:24px;border-radius:8px;height:max-content;position:sticky;top:88px}.orden-titulo{display:flex;justify-content:space-between;align-items:center}.orden-titulo h3{font-size:21px}.orden-titulo span{color:#79dd68;font-size:11px}.cliente{display:flex;align-items:center;gap:10px;background:#0f0f0f;border:1px solid #303030;border-radius:7px;padding:11px;margin:20px 0}.cliente input{border:0;background:transparent;color:#eee;width:100%;outline:0}.lineas article{display:grid;grid-template-columns:1fr auto;gap:12px;padding:17px 0;border-bottom:1px solid #303030}.lineas small{display:block;color:#999;margin-top:5px}.lineas b{grid-column:2;grid-row:1}.cantidad{display:flex;align-items:center;gap:10px}.cantidad button{width:25px;height:25px;border:0;background:#303030;color:#fff}.totales{padding:22px 0}.totales p{display:flex;justify-content:space-between;color:#aaa;margin:10px 0}.totales .total{color:#fff;border-top:1px solid #3b3b3b;padding-top:18px;font-size:21px}.cobrar,.cancelar{width:100%;border:0;border-radius:7px;padding:14px;font-weight:800}.cobrar{background:#107c10;color:#fff;display:flex;justify-content:center;align-items:center;gap:10px}.cancelar{background:transparent;color:#999;margin-top:7px}
@media(max-width:1100px){.pos-grid{grid-template-columns:1fr}.orden{position:static}.productos{grid-template-columns:repeat(2,1fr)}}@media(max-width:720px){.productos{grid-template-columns:1fr}.encabezado{align-items:start;gap:15px;flex-direction:column}.producto{grid-template-columns:100px 1fr}}
</style>
