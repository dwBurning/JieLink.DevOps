import Vue from 'vue'
import VueRouter from 'vue-router'
import Router from 'vue-router'
import Home from '@/components/Home'

import DemandManager from '@/components/DemandManager'
import DevOpsManager from '@/components/DevOpsManager'
import VersionManager from '@/components/VersionManager'
import PublishVersion from '@/components/PublishVersion'
import DevOpsProduct from '@/components/DevOpsProduct'
import Test from '@/components/Test'
const originalPush = VueRouter.prototype.push
VueRouter.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => err)
}

Vue.use(Router)

export default new Router({
  routes: [{
      path: '/',
      component: Home,
      name: '版本管理',
      iconCls: 'el-icon-upload',
      children: [
        {
          path: '/versionmanager',
          name: '工单管理',
          component: VersionManager,
          /*meta: {
            keepAlive: true
          }*/
        },{
          path: '/publishversion',
          name: '发布工单',
          component: PublishVersion
        },{
          path: '/test',
          name: '上传测试',
          component: Test
        }
      ]
    },{
      path: '/',
      component: Home,
      name: '运维管理',
      children: [
        {
          path: '/devopsmanager',
          iconCls: 'el-icon-first-aid-kit',
          name: '运维管理',
          component: DevOpsManager
        }
      ]
    },{
      path: '/',
      component: Home,
      name: '运维工具',
      children: [
        {
          path: '/devopsproduct',
          iconCls: 'el-icon-monitor',
          name: '运维工具',
          component: DevOpsProduct
        }
      ]
    }, {
      path: '/',
      component: Home,
      name: '需求管理',
      children: [
        {
          path: '/demandmanager',
          iconCls: 'el-icon-chat-dot-square',
          name: '需求管理',
          component: DemandManager
        }
      ]
    }
  ]
})
