<script setup>
import { computed, onBeforeUnmount, onMounted, reactive, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';
import { useNotificacionesStore } from '../../stores/notificaciones';
import {
  eliminarImagenProducto,
  subirImagenesProducto,
  validarImagenProducto,
} from '../../services/imagenesProducto';

const props = defineProps({ id: { type: String, default: null } });
const router = useRouter();
const datosStore = useDatosApiStore();
const avisos = useNotificacionesStore();
const { productos } = storeToRefs(datosStore);
const original = computed(() => productos.value.find((producto) => producto.id === Number(props.id)));
const editando = computed(() => Boolean(original.value));
const selectorImagen = ref(null);
const archivosImagen = ref([]);
const vistasPreviasLocales = ref([]);
const progreso = ref(0);
const guardando = ref(false);
const errorImagen = ref('');
const imagenesAEliminar = ref([]);
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
  imagenes: [],
  imagenUrl: '',
  imagenStoragePath: '',
});

watch(original, (producto) => {
  if (producto) Object.assign(formulario, producto);
}, { immediate: true });

onMounted(async () => {
  await datosStore.cargarRecurso('productos', productosApi);
  if (props.id) {
    try {
      Object.assign(formulario, await productosApi.obtenerPorId(props.id));
    } catch {
      errorImagen.value = 'No se pudo cargar el producto completo.';
    }
  }
});
onBeforeUnmount(() => liberarVistasPrevias());

const imagenesGuardadas = computed(() => formulario.imagenes?.length
  ? formulario.imagenes
  : (formulario.imagenUrl ? [{ url: formulario.imagenUrl, storagePath: formulario.imagenStoragePath }] : []));
const imagenesVista = computed(() => [
  ...imagenesGuardadas.value.map((imagen) => ({ ...imagen, local: false })),
  ...vistasPreviasLocales.value.map((url, indice) => ({ url, local: true, indice })),
]);

function liberarVistasPrevias() {
  vistasPreviasLocales.value.forEach((url) => URL.revokeObjectURL(url));
  vistasPreviasLocales.value = [];
}

function seleccionarImagen(evento) {
  errorImagen.value = '';
  const archivos = Array.from(evento.target.files || []);
  if (!archivos.length) return;
  try {
    if (imagenesVista.value.length >= 1 || archivos.length > 1) {
      throw new Error('Cada producto admite una sola imagen. Quita la actual para reemplazarla.');
    }
    const archivo = archivos[0];
    validarImagenProducto(archivo);
    archivosImagen.value = [archivo];
    vistasPreviasLocales.value = [URL.createObjectURL(archivo)];
  } catch (error) {
    evento.target.value = '';
    errorImagen.value = error.message;
  }
}

function quitarImagen(imagen) {
  if (imagen.local) {
    URL.revokeObjectURL(vistasPreviasLocales.value[imagen.indice]);
    vistasPreviasLocales.value.splice(imagen.indice, 1);
    archivosImagen.value.splice(imagen.indice, 1);
  } else {
    if (imagen.id) imagenesAEliminar.value.push(imagen.id);
    const indice = formulario.imagenes?.findIndex((item) => item.url === imagen.url);
    if (indice >= 0) formulario.imagenes.splice(indice, 1);
    else {
      formulario.imagenUrl = '';
      formulario.imagenStoragePath = '';
    }
  }
  if (selectorImagen.value) selectorImagen.value.value = '';
}

async function guardarProducto() {
  guardando.value = true;
  errorImagen.value = '';
  try {
    let guardado = await datosStore.guardarProducto({
      ...formulario,
      id: original.value?.id ?? 0,
    });

    await Promise.all(imagenesAEliminar.value.map((imagenId) =>
      eliminarImagenProducto(guardado.id, imagenId)));

    if (archivosImagen.value.length) {
      await subirImagenesProducto(guardado.id, archivosImagen.value, (valor) => {
        progreso.value = valor;
      });
    }

    avisos.mostrar(`Producto ${editando.value ? 'actualizado' : 'creado'} correctamente.`);
    router.push('/admin/productos');
  } catch (error) {
    errorImagen.value = error.message || 'No se pudo guardar el producto.';
  } finally {
    guardando.value = false;
    progreso.value = 0;
  }
}
</script>

<template>
  <AdminLayout :titulo="editando ? 'Editar producto' : 'Nuevo producto'">
    <template #header>
      <div class="cabecera">
        <div>
          <p class="eyebrow">Inventario / Productos</p>
          <h2>{{ editando ? 'Editar producto' : 'Nuevo producto' }}</h2>
        </div>
      </div>
    </template>
    <form class="form-grid" @submit.prevent="guardarProducto">
      <section class="panel core">
        <h3>Información principal</h3>
        <div class="campo full"><label>Nombre del producto</label><input v-model.trim="formulario.nombre" required minlength="2" maxlength="120" placeholder="Ej. Control inalámbrico Xbox" /></div>
        <div class="campo full"><label>Descripción</label><textarea v-model.trim="formulario.descripcion" rows="5" maxlength="1000" placeholder="Características, compatibilidad y especificaciones..."></textarea></div>
        <div class="campo"><label>Categoría</label><select v-model="formulario.categoria"><option>Consolas</option><option>Videojuegos</option><option>Accesorios</option></select></div>
        <div class="campo"><label>Marca</label><input v-model.trim="formulario.marca" required maxlength="80" placeholder="Ej. Microsoft" /></div>
      </section>
      <aside class="panel media">
        <h3>Imagen del producto</h3>
        <input ref="selectorImagen" class="selector-archivo" type="file" accept="image/jpeg,image/png,image/webp" @change="seleccionarImagen" />
        <button v-if="!imagenesVista.length" type="button" class="upload" @click="selectorImagen?.click()">
          <span class="material-symbols-outlined">add_photo_alternate</span>
          <strong>Agregar imagen</strong>
          <small>PNG, JPG o WebP (máx. 10 MB)</small>
        </button>
        <div v-if="imagenesVista.length" class="galeria-imagenes">
          <article v-for="imagen in imagenesVista" :key="imagen.url">
            <img :src="imagen.url" alt="Vista previa del producto" />
            <button type="button" aria-label="Quitar imagen" @click="quitarImagen(imagen)">
              <span class="material-symbols-outlined">delete</span>
            </button>
          </article>
        </div>
        <div v-if="guardando && progreso" class="progreso" role="status">
          <span :style="{ width: `${progreso}%` }"></span>
          <small>Subiendo imagen: {{ progreso }}%</small>
        </div>
        <p v-if="errorImagen" class="error-imagen">{{ errorImagen }}</p>
        <label class="toggle-row"><span class="material-symbols-outlined">visibility</span><span>Producto activo</span><input v-model="formulario.activo" type="checkbox" /></label>
        <label class="toggle-row"><span class="material-symbols-outlined">inventory</span><span>Controlar inventario</span><input type="checkbox" checked /></label>
      </aside>
      <section class="panel pricing">
        <h3>Inventario y precio</h3>
        <div class="pricing-grid">
          <div class="campo"><label>Precio de compra</label><input v-model.number="formulario.precioCompra" type="number" min="0" step=".01" required /></div>
          <div class="campo"><label>Precio de venta</label><input v-model.number="formulario.precioVenta" type="number" min="0.01" step=".01" required /></div>
          <div class="campo"><label>Cantidad disponible</label><input v-model.number="formulario.stock" type="number" min="0" step="1" required /></div>
          <div class="campo"><label>Stock mínimo</label><input v-model.number="formulario.stockMinimo" type="number" min="0" step="1" required /></div>
        </div>
      </section>
      <div class="acciones">
        <button type="button" class="btn-secondary" :disabled="guardando" @click="router.back()">Cancelar</button>
        <button class="btn-primary" :disabled="guardando">{{ guardando ? 'Guardando…' : 'Guardar producto' }}</button>
      </div>
    </form>
  </AdminLayout>
</template>

<style scoped>
.cabecera h2{font-size:30px}.eyebrow{font:500 11px 'JetBrains Mono';color:var(--color-primary);text-transform:uppercase;margin-bottom:5px}.form-grid{display:grid;grid-template-columns:minmax(0,2fr) minmax(280px,1fr);gap:24px}.panel{background:#191919;border:1px solid #242424;border-radius:10px;padding:28px}.panel h3{color:var(--color-primary);font-size:20px;margin-bottom:25px}.core{display:grid;grid-template-columns:1fr 1fr;gap:22px}.full{grid-column:1/-1}.media{grid-row:span 2;display:flex;flex-direction:column;gap:18px}.selector-archivo{display:none}.upload{position:relative;min-height:260px;overflow:hidden;border:2px dashed #303030;background:#141414;color:#eee;border-radius:10px;display:flex;flex-direction:column;align-items:center;justify-content:center;gap:9px;cursor:pointer}.upload:hover{border-color:var(--color-primary)}.upload img{position:absolute;inset:0;width:100%;height:100%;object-fit:cover}.upload.con-imagen::after{content:"";position:absolute;inset:0;background:linear-gradient(transparent 35%,rgba(0,0,0,.82))}.upload .material-symbols-outlined,.upload strong,.upload small{position:relative;z-index:1}.upload .material-symbols-outlined{font-size:45px}.upload small{color:#aaa}.quitar-imagen{display:flex;align-items:center;justify-content:center;gap:7px;padding:10px;border:1px solid #482b2b;background:#241616;color:#ff9d9d;border-radius:7px}.quitar-imagen .material-symbols-outlined{font-size:18px}.progreso{height:28px;position:relative;overflow:hidden;background:#101010;border-radius:5px}.progreso span{display:block;height:100%;background:var(--color-primary);transition:width .2s}.progreso small{position:absolute;inset:0;display:grid;place-items:center;color:#fff}.error-imagen{padding:10px;border:1px solid #5a2b2b;background:#2b1515;color:#ffaaaa;border-radius:6px;font-size:13px}.toggle-row{display:flex;align-items:center;gap:12px;padding:17px;background:#101010;border-radius:8px}.toggle-row input{margin-left:auto;accent-color:var(--color-primary)}.pricing-grid{display:grid;grid-template-columns:1fr 1fr;gap:24px}.acciones{grid-column:1/-1;border-top:1px solid #252525;padding-top:24px;display:flex;justify-content:flex-end;gap:14px}.acciones button:disabled{opacity:.55;cursor:wait}@media(max-width:900px){.form-grid{grid-template-columns:1fr}.media{grid-row:auto}.core{grid-template-columns:1fr}.full{grid-column:auto}.pricing-grid{grid-template-columns:1fr}}
.galeria-imagenes{display:grid;grid-template-columns:1fr}.galeria-imagenes article{position:relative;aspect-ratio:1;overflow:hidden;border-radius:8px;background:#101010}.galeria-imagenes img{width:100%;height:100%;object-fit:cover}.galeria-imagenes button{position:absolute;top:8px;right:8px;width:36px;height:36px;border:0;border-radius:50%;display:grid;place-items:center;background:rgba(30,8,8,.88);color:#ffaaaa}.galeria-imagenes button span{font-size:19px}
</style>
