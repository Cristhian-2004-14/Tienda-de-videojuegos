<script setup>
import { computed, onMounted, reactive, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useRouter } from 'vue-router';
import AdminLayout from '../../components/AdminLayout.vue';
import { useDatosApiStore } from '../../stores/datosApi';
import { clientesApi } from '../../services/recursosApi';

// Patrón Repository: se consulta el cliente mock sin acoplar la vista a una API.
const props = defineProps({ id: { type: String, default: null } });
const router = useRouter();
const datosStore = useDatosApiStore();
const { clientes } = storeToRefs(datosStore);
const original = computed(() => clientes.value.find((cliente) => cliente.id === Number(props.id)));
const editando = computed(() => Boolean(original.value));
const formulario = reactive({ nombre: '', apellido: '', telefono: '', email: '', notas: '' });

watch(original, (cliente) => {
  if (cliente) Object.assign(formulario, cliente);
}, { immediate: true });

onMounted(() => datosStore.cargarRecurso('clientes', clientesApi));
async function guardar() {
  await datosStore.guardarCliente({
    ...formulario,
    id: original.value?.id ?? 0,
  });
  router.push('/admin/clientes');
}
</script>

<template>
  <AdminLayout :titulo="editando ? 'Editar cliente' : 'Agregar cliente'">
    <template #header><div><p class="eyebrow">Clientes / Ficha</p><h2>{{ editando ? 'Editar cliente' : 'Agregar cliente' }}</h2><p class="sub">Completa los datos de contacto y guarda la ficha.</p></div></template>
    <form class="customer-form" @submit.prevent="guardar">
      <div class="avatar"><span class="material-symbols-outlined">person</span><button type="button">Agregar foto</button></div>
      <section>
        <h3>Información personal</h3>
        <div class="grid"><div class="campo"><label>Nombre</label><input v-model="formulario.nombre" required /></div><div class="campo"><label>Apellido</label><input v-model="formulario.apellido" required /></div><div class="campo"><label>Teléfono</label><input v-model="formulario.telefono" required /></div><div class="campo"><label>Correo electrónico</label><input v-model="formulario.email" type="email" required /></div><div class="campo full"><label>Notas</label><textarea v-model="formulario.notas" rows="4" placeholder="Preferencias, observaciones o información adicional..."></textarea></div></div>
      </section>
      <footer><button type="button" class="btn-secondary" @click="router.back()">Cancelar</button><button class="btn-primary">Guardar cliente</button></footer>
    </form>
  </AdminLayout>
</template>

<style scoped>
.eyebrow{font:500 11px 'JetBrains Mono';color:var(--color-primary);text-transform:uppercase}.sub{color:#9aa497;margin-top:7px}.customer-form{max-width:900px;background:#191919;border:1px solid #292929;border-radius:12px;padding:32px;display:grid;grid-template-columns:180px 1fr;gap:35px}.avatar{min-height:190px;border:1px dashed #3a4938;border-radius:10px;display:flex;flex-direction:column;align-items:center;justify-content:center;gap:15px}.avatar .material-symbols-outlined{font-size:70px;color:var(--color-primary)}.avatar button{border:0;background:none;color:#ddd}.customer-form h3{color:var(--color-primary);margin-bottom:25px}.grid{display:grid;grid-template-columns:1fr 1fr;gap:20px}.full{grid-column:1/-1}.customer-form footer{grid-column:1/-1;border-top:1px solid #2b2b2b;padding-top:24px;display:flex;justify-content:flex-end;gap:12px}@media(max-width:750px){.customer-form{grid-template-columns:1fr}.grid{grid-template-columns:1fr}.full,.customer-form footer{grid-column:auto}}
</style>
