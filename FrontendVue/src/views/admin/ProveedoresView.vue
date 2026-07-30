<script setup>
import { onMounted, reactive, ref } from 'vue';
import AdminLayout from '../../components/AdminLayout.vue';
import AdminPageHeader from '../../components/common/AdminPageHeader.vue';
import DataTable from '../../components/common/DataTable.vue';
import { proveedoresApi } from '../../services/recursosApi';

const proveedores = ref([]);
const editandoId = ref(null);
const guardando = ref(false);
const error = ref('');
const formulario = reactive({ razonSocial: '', nit: '', telefono: '', email: '', direccion: '', activo: true });
const vacio = { razonSocial: '', nit: '', telefono: '', email: '', direccion: '', activo: true };

async function cargar() {
  try { proveedores.value = await proveedoresApi.obtenerTodos(); }
  catch { error.value = 'No se pudieron cargar los proveedores.'; }
}
function limpiar() { editandoId.value = null; Object.assign(formulario, vacio); }
function editar(proveedor) {
  editandoId.value = proveedor.id;
  Object.assign(formulario, proveedor);
  window.scrollTo({ top: 0, behavior: 'smooth' });
}
async function guardar() {
  guardando.value = true; error.value = '';
  try {
    if (editandoId.value) await proveedoresApi.actualizar(editandoId.value, { ...formulario, id: editandoId.value });
    else await proveedoresApi.crear({ ...formulario });
    limpiar(); await cargar();
  } catch (e) { error.value = e.message || 'No se pudo guardar el proveedor.'; }
  finally { guardando.value = false; }
}
onMounted(cargar);
</script>

<template>
  <AdminLayout titulo="Proveedores">
    <template #header><AdminPageHeader eyebrow="INVENTARIO / PROVEEDORES" title="Proveedores" description="Datos de contacto para registrar el abastecimiento."><router-link class="btn-primary" to="/admin/compras">Ir a compras</router-link></AdminPageHeader></template>
    <form class="panel-caso formulario-grid" @submit.prevent="guardar">
      <div class="campo"><label>Razón social</label><input v-model.trim="formulario.razonSocial" required minlength="2" maxlength="120"></div>
      <div class="campo"><label>NIT</label><input v-model.trim="formulario.nit" minlength="5" maxlength="20" pattern="[A-Za-z0-9-]*" title="Usa letras, números o guiones."></div>
      <div class="campo"><label>Teléfono</label><input v-solo-digitos v-model.trim="formulario.telefono" type="tel" minlength="7" maxlength="15" pattern="[0-9]{7,15}" inputmode="numeric" title="Ingresa solamente entre 7 y 15 números."></div>
      <div class="campo"><label>Correo</label><input v-model.trim="formulario.email" type="email" maxlength="120"></div>
      <div class="campo campo-ancho"><label>Dirección</label><input v-model.trim="formulario.direccion" maxlength="250"></div>
      <label class="check"><input v-model="formulario.activo" type="checkbox"> Proveedor activo</label>
      <div class="acciones"><button class="btn-primary" :disabled="guardando">{{ guardando ? 'Guardando...' : editandoId ? 'Actualizar proveedor' : 'Agregar proveedor' }}</button><button v-if="editandoId" class="btn-secondary" type="button" @click="limpiar">Cancelar</button></div>
      <p v-if="error" class="error" role="alert">{{ error }}</p>
    </form>
    <section class="panel-caso"><DataTable :empty="!proveedores.length" empty-text="No hay proveedores registrados" :columns="5"><template #header><thead><tr><th>Proveedor</th><th>NIT</th><th>Contacto</th><th>Estado</th><th>Acción</th></tr></thead></template><tr v-for="p in proveedores" :key="p.id"><td>{{ p.razonSocial }}</td><td>{{ p.nit || '—' }}</td><td>{{ p.telefono || p.email || '—' }}</td><td><span class="estado-caso">{{ p.activo ? 'Activo' : 'Inactivo' }}</span></td><td><button class="accion-caso boton-link" @click="editar(p)">Editar</button></td></tr></DataTable></section>
  </AdminLayout>
</template>

<style scoped>
.formulario-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:16px}.campo-ancho,.acciones,.error{grid-column:1/-1}.check{display:flex;gap:8px;align-items:center}.acciones{display:flex;gap:10px}.boton-link{background:none;border:0;cursor:pointer}.error{color:#ffb4ab}@media(max-width:650px){.formulario-grid{grid-template-columns:1fr}.campo-ancho,.acciones,.error{grid-column:auto}}
</style>
