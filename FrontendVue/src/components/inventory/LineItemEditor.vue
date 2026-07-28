<script setup>
const props=defineProps({
  modelValue:{type:Array,required:true},
  products:{type:Array,default:()=>[]},
  priceLabel:{type:String,default:'Precio unitario'},
  autoPrice:{type:Boolean,default:false},
  allowEmpty:{type:Boolean,default:false},
});
const emit=defineEmits(['update:modelValue']);
function update(index,field,value){
  emit('update:modelValue',props.modelValue.map((line,i)=>i===index?{...line,[field]:value}:line));
}
function selectProduct(index,value){
  const product=props.products.find(item=>item.id===Number(value));
  emit('update:modelValue',props.modelValue.map((line,i)=>i===index?{
    ...line,
    productoId:Number(value),
    precioUnitario:props.autoPrice&&product?product.precioVenta:line.precioUnitario,
  }:line));
}
function add(){emit('update:modelValue',[...props.modelValue,{productoId:'',cantidad:1,precioUnitario:0}])}
function remove(index){
  const lines=props.modelValue.filter((_,i)=>i!==index);
  emit('update:modelValue',lines.length||props.allowEmpty?lines:[{productoId:'',cantidad:1,precioUnitario:0}]);
}
</script>
<template><div class="editor"><article v-for="(line,index) in modelValue" :key="index" class="line"><div class="campo product"><label>Producto</label><select :value="line.productoId" required @change="selectProduct(index,$event.target.value)"><option value="">Seleccionar</option><option v-for="product in products" :key="product.id" :value="product.id">{{product.nombre}} (stock: {{product.stock}})</option></select></div><div class="campo"><label>Cantidad</label><input :value="line.cantidad" type="number" min="1" required @input="update(index,'cantidad',Number($event.target.value))"></div><div class="campo"><label>{{priceLabel}}</label><input :value="line.precioUnitario" type="number" min=".01" step=".01" required @input="update(index,'precioUnitario',Number($event.target.value))"></div><button class="remove" type="button" aria-label="Quitar producto" @click="remove(index)">×</button></article><button class="accion-caso add" type="button" @click="add">+ Agregar otro producto</button></div></template>
<style scoped>.editor{display:grid;gap:14px}.line{display:grid;grid-template-columns:minmax(220px,2fr) 1fr 1fr auto;gap:12px;align-items:end;padding:14px;background:#171717;border:1px solid #333;border-radius:8px}.remove{height:42px;width:42px;border:1px solid #5c3535;background:#291919;color:#ffb4ab;border-radius:7px;font-size:22px}.add{justify-self:start;background:none;border:0;cursor:pointer}@media(max-width:750px){.line{grid-template-columns:1fr 1fr}.product{grid-column:1/-1}}</style>
