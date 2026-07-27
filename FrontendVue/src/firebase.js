import { initializeApp } from 'firebase/app';
import { getAnalytics, isSupported } from 'firebase/analytics';

const firebaseConfig = {
  apiKey: 'AIzaSyA_qzdbrysk3cU-HZhX_yqBM9Pk6kBpZt4',
  authDomain: 'tienda-83288.firebaseapp.com',
  projectId: 'tienda-83288',
  storageBucket: 'tienda-83288.firebasestorage.app',
  messagingSenderId: '402091592604',
  appId: '1:402091592604:web:5ee04663ff99ad993d9659',
  measurementId: 'G-780S37Q7R6',
};

export const firebaseApp = initializeApp(firebaseConfig);

export async function inicializarAnalytics() {
  if (await isSupported()) getAnalytics(firebaseApp);
}
