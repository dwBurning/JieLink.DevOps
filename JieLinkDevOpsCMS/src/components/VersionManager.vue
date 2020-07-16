<template>
    <div id="app">
        <Header></Header>
        <div id="v-content" v-bind:style="{minHeight: Height+'px'}"><router-view /></div>
        <Footer>由jielink2.*团队提供技术支持</Footer>
    </div>
</template>

<script>
export default {
  name: 'App',
  data() {
     return {
       Height: 0
     }
  },
  mounted(){
    //动态设置内容高度 让footer始终居底   header+footer的高度是200
    this.Height = document.documentElement.clientHeight - 200;  
　　//监听浏览器窗口变化　
    window.onresize = ()=> {this.Height = document.documentElement.clientHeight -200}
  }
}
</script>