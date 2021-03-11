import Vue from 'vue'
import VueRouter from 'vue-router'
import Router from 'vue-router'
import Home from '@/components/Home'

import DemandManager from '@/components/DemandManager'
import DevOpsManager from '@/components/DevOpsManager'
import VersionManager from '@/components/VersionManager'
import VersionDownloadManager from '@/components/VersionDownloadManager'
import PublishVersion from '@/components/PublishVersion'
import DevOpsProduct from '@/components/DevOpsProduct'
import Lonin from '@/components/Login'
import UserManager from '@/components/UserManager'
import VerifyBillManager from '@/components/VerifyBillManager'
import VerifyBillPublish from '@/components/VerifyBillPublish'

const originalPush = VueRouter.prototype.push
VueRouter.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => err)
}

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: '登录',
      component: Lonin,
      hidden: true,
      meta: { role: ['admin', 'other'] },
    }
    , {
      path: '/home',
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
        }, {
          path: '/versiondownloadmanager',
          name: '下载详情',
          component: VersionDownloadManager,
          hidden: true,
        },{
          path: '/publishversion',
          name: '发布工单',
          component: PublishVersion,
          hidden: true,
        }
      ]
    }, {
      path: '/home',
      component: Home,
      name: '运维管理-1',
      children: [
        {
          path: '/devopsmanager',
          iconCls: 'el-icon-first-aid-kit',
          name: '运维事件',
          component: DevOpsManager,
          hidden: true,
        }
      ]
    }, {
      path: '/home',
      component: Home,
      name: '运维工具-1',
      children: [
        {
          path: '/devopsproduct',
          iconCls: 'el-icon-monitor',
          name: '运维工具',
          component: DevOpsProduct,
        }
      ]
    }, {
      path: '/home',
      component: Home,
      name: '需求管理-1',
      children: [
        {
          path: '/demandmanager',
          iconCls: 'el-icon-chat-dot-square',
          name: '需求管理',
          component: DemandManager,
        }
      ]
    }, {
      path: '/home',
      component: Home,
      name: '用户管理-1',
      children: [
        {
          path: '/userManager',
          iconCls: 'el-icon-user',
          name: '用户管理',
          component: UserManager,
          hidden: true,
        }
      ]
    }, {
      path: '/home',
      component: Home,
      name: '对账系统',
      iconCls: 'el-icon-document-checked',
      children: [
        {
          path: '/VerifyBillPublish',
          name: '对账处理',
          component: VerifyBillPublish,
          hidden: true,
        }
        ,{
          path: '/VerifyBillManager',
          name: '建设中',
          component: VerifyBillManager,
          hidden: true,
        }
      ]
    }
  ]
})
