<script setup>
import { computed, onMounted, reactive, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';
import { useNotificacionesStore } from '../../stores/notificaciones';

// Patrón Repository: la vista carga el producto desde la capa mock src/data.
const props = defineProps({ id: { type: String, default: null } });
const router = useRouter();
const datosStore = useDatosApiStore();
const avisos = useNotificacionesStore();
const { productos } = storeToRefs(datosStore);
const original = computed(() => productos.value.find((producto) => producto.id === Number(props.id)));
const editando = computed(() => Boolean(original.value));
const formulario = reactive({
  nombre: '',
  categoria: 'Consolas',
  marca: '',
  precioCompra: 0,
  precioVenta: 0,
  stock: 0,
  stockMinimo: 5,
  activo: true,
  descripcion: '',
});

watch(original, (producto) => {
  if (producto) Object.assign(formulario, producto);
}, { immediate: true });

onMounted(() => datosStore.cargarRecurso('productos', productosApi));

async function guardarProducto() {
  await datosStore.guardarProducto({
    ...formulario,
    id: original.value?.id ?? 0,
  });
  avisos.mostrar(`Producto ${editando.value ? 'actualizado' : 'creado'} correctamente.`);
  router.push('/admin/productos');
}
</script>

<template>
  <AdminLayout :titulo="editando ? 'Editar producto' : 'Nuevo producto'">
    <template #header>
      <div class="cabecera"><div><p class="eyebrow">Inventario / Productos</p><h2>{{ editando ? 'Editar producto' : 'Nuevo producto' }}</h2></div></div>
    </template>
    <form class="form-grid" @submit.prevent="guardarProducto">
      <section class="panel core">
        <h3>Información principal</h3>
        <div class="campo full"><label>Nombre del producto</label><input v-model="formulario.nombre" required placeholder="Ej. Control inalámbrico Xbox" /></div>
        <div class="campo full"><label>Descripción</label><textarea v-model="formulario.descripcion" rows="5" placeholder="Características, compatibilidad y especificaciones..."></textarea></div>
        <div class="campo"><label>Categoría</label><select v-model="formulario.categoria"><option>Consolas</option><option>Videojuegos</option><option>Accesorios</option></select></div>
        <div class="campo"><label>Marca</label><input v-model="formulario.marca" required placeholder="Ej. Microsoft" /></div>
      </section>
      <aside class="panel media">
        <h3>Imagen del producto</h3>
        <button type="button" class="upload"><span class="material-symbols-outlined">add_photo_alternate</span><strong>Subir imagen</strong><small>PNG, JPG o WebP (máx. 5 MB)</small></button>
        <label class="toggle-row"><span class="material-symbols-outlined">visibility</span><span>Producto activo</span><input v-model="formulario.activo" type="checkbox" /></label>
        <label class="toggle-row"><span class="material-symbols-outlined">inventory</span><span>Controlar inventario</span><input type="checkbox" checked /></label>
      </aside>
      <section class="panel pricing">
        <h3>Inventario y precio</h3>
        <div class="pricing-grid">
          <div class="campo"><label>Precio de compra</label><input v-model.number="formulario.precioCompra" type="number" min="0" step=".01" required /></div>
          <div class="campo"><label>Precio de venta</label><input v-model.number="formulario.precioVenta" type="number" min="0" step=".01" required /></div>
          <div class="campo"><label>Cantidad disponible</label><input v-model.number="formulario.stock" type="number" min="0" required /></div>
          <div class="campo"><label>Stock mínimo</label><input v-model.number="formulario.stockMinimo" type="number" min="0" required /></div>
        </div>
      </section>
      <div class="acciones"><button type="button" class="btn-secondary" @click="router.back()">Cancelar</button><button class="btn-primary">Guardar producto</button></div>
    </form>
  </AdminLayout>
</template>

<style scoped>
.cabecera h2{font-size:30px}.eyebrow{font:500 11px 'JetBrains Mono';color:var(--color-primary);text-transform:uppercase;margin-bottom:5px}.form-grid{display:grid;grid-template-columns:minmax(0,2fr) minmax(280px,1fr);gap:24px}.panel{background:#191919;border:1px solid #242424;border-radius:10px;padding:28px}.panel h3{color:var(--color-primary);font-size:20px;margin-bottom:25px}.core{display:grid;grid-template-columns:1fr 1fr;gap:22px}.full{grid-column:1/-1}.media{grid-row:span 2;display:flex;flex-direction:column;gap:18px}.upload{min-height:260px;border:2px dashed #303030;background:#141414;color:#eee;border-radius:10px;display:flex;flex-direction:column;align-items:center;justify-content:center;gap:9px}.upload .material-symbols-outlined{font-size:45px}.upload small{color:#777}.toggle-row{display:flex;align-items:center;gap:12px;padding:17px;background:#101010;border-radius:8px}.toggle-row input{margin-left:auto;accent-color:var(--color-primary)}.pricing-grid{display:grid;grid-template-columns:1fr 1fr;gap:24px}.acciones{grid-column:1/-1;border-top:1px solid #252525;padding-top:24px;display:flex;justify-content:flex-end;gap:14px}@media(max-width:900px){.form-grid{grid-template-columns:1fr}.media{grid-row:auto}.core{grid-template-columns:1fr}.full{grid-column:auto}.pricing-grid{grid-template-columns:1fr}}
</style>
