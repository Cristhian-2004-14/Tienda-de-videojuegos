<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { storeToRefs } from 'pinia';
import TiendaLayout from '../../components/TiendaLayout.vue';
import ProductoVisual from '../../components/ProductoVisual.vue';
import { useCarritoStore } from '../../stores/carrito';
import { useDatosApiStore } from '../../stores/datosApi';
import { productosApi } from '../../services/recursosApi';
const carrito=useCarritoStore(); const {items,cantidadItems,total}=storeToRefs(carrito);
const datosStore=useDatosApiStore(); const {productos}=storeToRefs(datosStore);
const contacto=reactive({nombre:'',telefono:'',nota:''}); const numeroWhatsApp='59169078166';
const formularioContacto=ref(null);
const telefonoContacto=ref(null);
const errorContacto=ref('');
const mensaje=computed(()=>[`Hola, quiero hacer este pedido:`,...items.value.map(i=>`• ${i.nombre} x${i.cantidad} - $${(i.precioUnitario*i.cantidad).toFixed(2)}`),`Total referencial: $${total.value.toFixed(2)}`,`Nombre: ${contacto.nombre}`,`Teléfono: ${contacto.telefono}`,contacto.nota&&`Nota: ${contacto.nota}`].filter(Boolean).join('\n'));
const enlace=computed(()=>`https://wa.me/${numeroWhatsApp}?text=${encodeURIComponent(mensaje.value)}`);
const productoVisual=(item)=>productos.value.find((producto)=>producto.id===item.productoId)||item;
function cambiarCantidadEscrita(item, evento) {
  const cantidad = Number(evento.target.value);
  if (!Number.isInteger(cantidad) || cantidad < 1) {
    evento.target.value = item.cantidad;
    return;
  }
  if (!carrito.cambiarCantidad(item.productoId, cantidad)) {
    evento.target.value = item.cantidad;
  }
}
function escribirTelefono(evento) {
  const telefono = evento.target.value.replace(/\D/g, '').slice(0, 15);
  contacto.telefono = telefono;
  evento.target.value = telefono;
  errorContacto.value = '';
}
function enviarPedido() {
  errorContacto.value = '';
  if (!/^[0-9]{7,15}$/.test(contacto.telefono)) {
    errorContacto.value = 'El teléfono debe contener solamente entre 7 y 15 números.';
    telefonoContacto.value?.focus();
    return;
  }
  if (!/^[\p{L}][\p{L}\s'.-]{1,79}$/u.test(contacto.nombre)) {
    errorContacto.value = 'Ingresa un nombre válido usando letras y espacios.';
    return;
  }
  if (!formularioContacto.value?.reportValidity()) {
    errorContacto.value = 'Revisa los datos de contacto antes de continuar.';
    return;
  }
  window.open(enlace.value, '_blank', 'noopener,noreferrer');
}
onMounted(async()=>{await datosStore.cargarRecurso('productos',productosApi);carrito.sincronizarStock(productos.value)});
</script>
<template><TiendaLayout><main class="checkout"><p class="pasos mono"><b>01 SELECCIÓN</b><span></span>02 DATOS<span></span>03 WHATSAPP</p><h1>Tu selección de productos</h1><p class="aclaracion">Este carrito no procesa pagos. Úsalo para preparar tu pedido y consultar disponibilidad por WhatsApp.</p>
<div v-if="items.length" class="checkout-grid"><section><h2>Productos seleccionados <small>({{cantidadItems}})</small></h2><div class="items"><article v-for="item in items" :key="item.productoId"><div class="item-visual"><ProductoVisual :producto="productoVisual(item)"/></div><div class="item-info"><small>{{item.categoria}}</small><h3>{{item.nombre}}</h3><p>{{item.marca}} · {{item.stockDisponible}} disponibles</p><button @click="carrito.quitarProducto(item.productoId)">Eliminar</button></div><div class="item-precio"><strong>${{item.precioUnitario.toFixed(2)}}</strong><div class="control-cantidad"><button type="button" aria-label="Disminuir cantidad" @click="carrito.cambiarCantidad(item.productoId,item.cantidad-1)">−</button><input :value="item.cantidad" type="number" min="1" :max="item.stockDisponible" step="1" inputmode="numeric" :aria-label="`Cantidad de ${item.nombre}`" @change="cambiarCantidadEscrita(item,$event)"><button type="button" aria-label="Aumentar cantidad" :disabled="item.cantidad>=item.stockDisponible" title="Límite según stock disponible" @click="carrito.cambiarCantidad(item.productoId,item.cantidad+1)">+</button></div></div></article></div><router-link to="/tienda" class="seguir">← Seguir seleccionando</router-link></section>
<aside><div class="resumen"><h2>Resumen referencial</h2><p><span>Productos</span><strong>{{cantidadItems}}</strong></p><p class="total"><span>Total estimado</span><strong>${{total.toFixed(2)}}</strong></p><small>El negocio confirmará stock, entrega y precio final por WhatsApp.</small></div><form ref="formularioContacto" @submit.prevent="enviarPedido"><h3>Datos de contacto</h3><label>Nombre<input v-model.trim="contacto.nombre" required minlength="2" maxlength="80" pattern="[\p{L}][\p{L}\s'.-]*" title="Ingresa un nombre válido usando letras y espacios." autocomplete="name"></label><label>Teléfono<input ref="telefonoContacto" :value="contacto.telefono" type="tel" required minlength="7" maxlength="15" pattern="[0-9]{7,15}" inputmode="numeric" title="Ingresa solamente entre 7 y 15 números." autocomplete="tel" @input="escribirTelefono"></label><small class="ayuda-telefono">Solo números, sin espacios ni letras.</small><label>Nota opcional<textarea v-model.trim="contacto.nota" rows="3" maxlength="300"></textarea></label><p v-if="errorContacto" class="error-contacto" role="alert">{{errorContacto}}</p><button class="whatsapp" type="submit"><span class="material-symbols-outlined">chat</span>Enviar pedido por WhatsApp</button></form></aside></div>
<section v-else class="vacio"><span class="material-symbols-outlined">shopping_bag</span><h2>Aún no seleccionaste productos</h2><p>Explora el catálogo y agrega lo que te interese.</p><router-link class="btn-primary" to="/tienda">Ver catálogo</router-link></section></main></TiendaLayout></template>
<style scoped>
.checkout{max-width:1280px;margin:auto;padding:45px clamp(20px,5vw,70px)}.pasos{display:flex;align-items:center;gap:12px;color:#626762;font-size:9px}.pasos b{color:#79dd68}.pasos span{height:1px;width:38px;background:#373737}.checkout>h1{font-size:42px;margin:20px 0 10px}.aclaracion{color:#9ba397;line-height:1.6;margin-bottom:40px}.checkout-grid{display:grid;grid-template-columns:minmax(0,1.2fr) minmax(350px,.8fr);gap:60px}.checkout h2{font-size:20px;margin-bottom:19px}.items{border-top:1px solid #313131}.items article{display:grid;grid-template-columns:120px 1fr auto;gap:20px;align-items:center;padding:20px 0;border-bottom:1px solid #313131}.item-visual{width:120px;height:120px;overflow:hidden;background:#202020;border-radius:8px}.item-visual :deep(.visual){height:100%;min-height:0}.item-info small{color:#79dd68}.item-info h3{margin:7px 0}.item-info p{color:#888}.item-info button{border:0;background:transparent;color:#aaa;text-decoration:underline;padding-top:15px}.item-precio{text-align:right}.item-precio>div{display:flex;margin-top:30px}.item-precio button,.item-precio span{width:29px;height:29px;border:1px solid #353535;background:#1d1d1d;color:#fff}.seguir{display:inline-block;color:#79dd68;margin-top:22px}.resumen,form{background:#1a1a1a;padding:25px}.resumen p{display:flex;justify-content:space-between;color:#aaa;margin:13px 0}.resumen .total{border-top:1px solid #363636;padding-top:20px;font-size:20px}.resumen .total strong{color:#79dd68}.resumen small{color:#777}form{margin-top:2px}form h3{margin-bottom:18px}form label{display:flex;flex-direction:column;gap:7px;font-size:10px;color:#91998e;margin-bottom:13px;text-transform:uppercase}form input,form textarea{background:#101010;border:1px solid #303030;color:#eee;padding:12px}.whatsapp{display:flex;justify-content:center;gap:8px;background:#25d366;color:#061c0e;padding:15px;border-radius:6px;font-weight:900}.disabled{opacity:.4;pointer-events:none}.vacio{text-align:center;padding:80px 20px;background:#191919}.vacio>span{font-size:48px;color:#79dd68}.vacio h2{margin:16px}.vacio p{color:#888;margin-bottom:28px}@media(max-width:900px){.checkout-grid{grid-template-columns:1fr}}@media(max-width:550px){.items article{grid-template-columns:85px 1fr;align-items:start}.item-visual{width:85px;height:85px}.item-precio{grid-column:2;display:flex;justify-content:space-between}.item-precio>div{margin:0}}
.control-cantidad{display:grid!important;grid-template-columns:42px 64px 42px;gap:5px;margin-top:24px!important;padding:5px;border:1px solid #303a30;border-radius:9px;background:#111511;box-shadow:inset 0 1px #ffffff08}
.control-cantidad button,.control-cantidad input{width:100%!important;height:40px!important;border:1px solid #394339!important;border-radius:6px!important;background:#202620!important;color:#fff!important;text-align:center;font:800 16px 'Inter',sans-serif;transition:.2s}
.control-cantidad button{display:grid;place-items:center;padding:0;color:#9ff18f!important}
.control-cantidad button:hover:not(:disabled){border-color:#79dd68!important;background:#193319!important;transform:translateY(-1px)}
.control-cantidad button:active:not(:disabled){transform:translateY(0) scale(.96)}
.control-cantidad button:disabled{opacity:.32;cursor:not-allowed}
.control-cantidad input{outline:none;appearance:textfield;-moz-appearance:textfield}
.control-cantidad input::-webkit-inner-spin-button,.control-cantidad input::-webkit-outer-spin-button{margin:0;appearance:none}
.control-cantidad input:focus{border-color:#79dd68!important;box-shadow:0 0 0 3px #79dd6818}
.whatsapp{width:100%;border:0;cursor:pointer;font:900 14px 'Inter',sans-serif}
.whatsapp:hover{background:#37e477;transform:translateY(-1px)}
.whatsapp:active{transform:translateY(0)}
.ayuda-telefono{display:block;margin:-7px 0 14px;color:#7f897d;font-size:10px}
.error-contacto{margin:0 0 13px;padding:10px 12px;border:1px solid #6a3535;border-radius:6px;background:#2a1717;color:#ffb4ab;font-size:12px;line-height:1.45}
@media(max-width:550px){.control-cantidad{grid-template-columns:38px 58px 38px;margin-top:0!important}.control-cantidad button,.control-cantidad input{height:38px!important}}
</style>
