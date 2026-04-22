import Main from '@/components/main'
import parentView from '@/components/parent-view'
import Print from '@/view/print'

// import '' from '@/view/components/organization/organization_details'

/**
 * iview-admin中meta除了原生参数外可配置的参数:
 * meta: {
 *  hideInMenu: (false) 设为true后在左侧菜单不会显示该页面选项
 *  notCache: (false) 设为true后页面不会缓存
 *  access: (null) 可访问该页面的权限数组，当前路由设置的权限会影响子路由
 *  icon: (-) 该页面在左侧菜单、面包屑和标签导航处显示的图标，如果是自定义图标，需要在图标名称前加下划线'_'
 *  beforeCloseName: (-) 设置该字段，则在关闭当前tab页时会去'@/router/before-close.js'里寻找该字段名对应的方法，作为关闭前的钩子函数
 * }
 */

export default [
  {
    path: '/login',
    name: 'login',
    meta: {
      title: 'Login - 登录',
      hideInMenu: true
    },
    component: () => import('@/view/login/login')
  },
  {
    path: '/',
    name: '_home',
    redirect: '/home',
    component: Main,
    meta: {
      hideInMenu: true,
      notCache: true
    },
    children: [
      {
        path: '/home',
        name: 'home',
        meta: {
          hideInMenu: true,
          title: '首页',
          notCache: true,
          icon: 'md-home'
        },
        component: () => import('@/view/home')
      }
    ]
  },
  // 机构管理
  {
    path: '/organization',
    name: 'organization',
    component: Main,
    meta: {
      // icon: 'ios-home-outline',
      icon: '_iconfont icon-jigou',
      title: '机构管理'
    },
    children: [
      {
        path: '/organization_chart',
        name: 'organization_chart',
        meta: {
          icon: '_iconfont icon-zuzhijiagou',
          title: '连锁机构'
        },
        component: () =>
          import('@/view/components/organization/organization_chart')
      },
      {
        path: 'staff',
        name: 'staff',
        meta: {
          icon: '_iconfont icon-people',
          title: '职工管理'
        },
        component: () => import('@/view/components/organization/staff/staff')
      },
      {
        path: '/organization_chart/organization_details/:id',
        name: 'organization_details',
        meta: {
          title: '机构详情',
          hideInMenu: true
        },
        component: () =>
          import('@/view/components/organization/organization_details')
      },
      {
        path: 'organization_statistics',
        name: 'organization_statistics',
        meta: {
          icon: '_iconfont icon-jigoutongji',
          title: '机构统计'
        },
        component: () =>
          import('@/view/components/organization/organization_statistics')
      }
    ]
  },
  // 运行管理
  {
    path: '/operation_management',
    name: 'operation_management',
    component: Main,
    meta: {
      icon: '_iconfont icon-yunhang',
      title: '运行管理'
    },
    children: [
      {
        path: 'patient',
        name: 'patient',
        meta: {
          icon: '_iconfont icon-huanzheguanli',
          title: '患者信息'
        },
        component: () =>
          import('@/view/components/operation_management/patient/patient')
      },
      {
        path: 'equipment',
        name: 'equipment',
        meta: {
          icon: '_iconfont icon-shebeiguanli',
          title: '设备管理'
        },
        component: () =>
          import('@/view/components/operation_management/equipment/equipment')
      },
      {
        path: 'purchase_requisition_form',
        name: 'purchase_requisition_form',
        meta: {
          icon: 'ios-copy-outline',
          title: '采购申请单'
        },
        component: () =>
          import(
            '@/view/components/operation_management/purchase_requisition_form/purchase_requisition_form'
          )
      },
      {
        path: 'purchase_requisitions',
        name: 'purchase_requisitions',
        meta: {
          icon: 'ios-copy-outline',
          title: '采购订单'
        },
        component: () =>
          import(
            '@/view/components/operation_management/purchase_requisitions/purchase_requisitions'
          )
      },
      {
        path: 'return_order',
        name: 'return_order',
        meta: {
          icon: 'ios-copy-outline',
          title: '退货订单'
        },
        component: () =>
          import(
            '@/view/components/operation_management/return_order/return_order'
          )
      },
      {
        path: 'medical',
        name: 'medical',
        meta: {
          // icon: 'ios-paper-outline',
          icon: '_iconfont icon-binganguanli',
          title: '病案管理'
        },
        component: () =>
          import('@/view/components/operation_management/medical/medical')
      },
      {
        path: 'quality_management',
        name: 'quality_management',
        component: parentView,
        meta: {
          icon: '_iconfont icon-zhiliangguanli',
          title: '质量管理'
        },
        children: [
          {
            path: 'medical_quality_statistics_y',
            name: 'medical_quality_statistics_y',
            meta: {
              title: '年度指标'
            },
            component: () =>
              import(
                '@/view/components/quality_management/medical_quality_statistics/medical_quality_statistics_y'
              )
          },
          {
            path: 'medical_quality_statistics_m',
            name: 'medical_quality_statistics_m',
            meta: {
              title: '月度指标'
            },
            component: () =>
              import(
                '@/view/components/quality_management/medical_quality_statistics/medical_quality_statistics_m'
              )
          },
          {
            path: 'process_quality_control',
            name: 'process_quality_control',
            meta: {
              title: '过程质控'
            },
            component: () =>
              import(
                '@/view/components/quality_management/process_quality_control/process_quality_control'
              )
          },
          {
            path: 'results_quality_control',
            name: 'results_quality_control',
            meta: {
              title: '结果质控'
            },
            component: () =>
              import(
                '@/view/components/quality_management/results_quality_control/results_quality_control'
              )
          }
        ]
      }
    ]
  },
  // 经营管理
  {
    path: '/business_management',
    name: 'business_management',
    component: Main,
    meta: {
      icon: '_iconfont icon-jingyingguanli',
      title: '经营管理'
    },
    children: [
      {
        path: 'financial',
        name: 'financial',
        meta: {
          icon: '_iconfont icon-caiwuguanli',
          title: '财务管理'
        },
        component: parentView,
        children: [
          {
            path: 'incoming_outgoings',
            name: 'incoming_outgoings',
            meta: {
              title: '收支报表'
            },
            component: () =>
              import(
                '@/view/components/business_management/financial/incoming_outgoings'
              )
          },
          {
            path: 'statistical',
            name: 'statistical',
            meta: {
              title: '统计分析'
            },
            component: () =>
              import(
                '@/view/components/business_management/financial/statistical'
              )
          },
          {
            path: 'profit',
            name: 'profit',
            meta: {
              title: '盈利分析'
            },
            component: () =>
              import('@/view/components/business_management/financial/profit')
          }
        ]
      },
      {
        path: 'supplies',
        name: 'supplies',
        meta: {
          icon: '_iconfont icon-wuziguanli',
          title: '物资管理'
        },
        component: parentView,
        children: [
          {
            path: 'material',
            name: 'material',
            meta: {
              title: '物资报表'
            },
            component: () =>
              import('@/view/components/business_management/supplies/material')
          },
          {
            path: 'statistics',
            name: 'statistics',
            meta: {
              title: '物资统计'
            },
            component: () =>
              import(
                '@/view/components/business_management/supplies/statistics'
              )
          }
        ]
      },
      {
        path: 'energy_management',
        name: 'energy_management',
        meta: {
          icon: '_iconfont icon-nenghaoguanli',
          title: '能耗管理'
        },
        component: () =>
          import(
            '@/view/components/business_management/energy/energy_management'
          )
      }
    ]
  },
  // 系统管理
  {
    path: '/system_management',
    name: 'system_management',
    component: Main,
    meta: {
      // icon: 'ios-settings-outline',
      icon: '_iconfont icon-xitongguanli',
      title: '系统管理'
    },
    children: [
      {
        path: 'user_manage',
        name: 'user_manage',
        meta: {
          icon: '_iconfont icon-yonghu',
          title: '用户管理'
        },
        component: () =>
          import('@/view/components/system_management/user-manage/user-manage')
      },
      {
        path: 'role_permissions',
        name: 'role_permissions',
        meta: {
          icon: '_iconfont icon-jiaosequanxian',
          title: '角色权限'
        },
        component: () =>
          import(
            '@/view/components/system_management/role-permissions/role-permissions'
          )
      },
      {
        path: 'role_manage',
        name: 'role_manage',
        meta: {
          icon: '_iconfont icon-jiaoseguanli',
          title: '角色管理'
        },
        component: () =>
          import('@/view/components/system_management/role-manage/role-manage')
      },
      {
        path: 'data_dictionary',
        name: 'data_dictionary',
        meta: {
          icon: '_iconfont icon-shujuzidian',
          title: '数据字典'
        },
        component: () =>
          import(
            '@/view/components/system_management/data-dictionary/data-dictionary'
          )
      },
      {
        path: 'system_log',
        name: 'system_log',
        meta: {
          icon: '_iconfont icon-rizhiyuncunyemian-xitongrizhi-',
          title: '系统日志'
        },
        component: () =>
          import('@/view/components/system_management/system-log/system-log')
      },
      {
        path: 'menu_manage',
        name: 'menu_manage',
        meta: {
          icon: '_iconfont icon-caidanguanli',
          title: '菜单管理'
        },
        component: () =>
          import('@/view/components/system_management/menu-manage/menu-manage')
      },
      {
        path: 'button_manage',
        name: 'button_manage',
        meta: {
          icon: '_iconfont icon-hw_icon-anniuguanli',
          title: '按钮管理'
        },
        component: () =>
          import(
            '@/view/components/system_management/button-manage/button-manage'
          )
      },
      {
        path: 'message_center',
        name: 'message_center',
        redirect: { name: 'notify' },
        meta: {
          icon: 'ios-notifications-outline',
          title: '消息中心'
        },
        component: () =>
          import(
            '@/view/components/system_management/message_center/message_center'
          ),
        children: [
          {
            path: 'approve',
            name: 'approve',
            meta: {
              hideInMenu: true,
              title: '审批'
            },
            component: () =>
              import(
                '@/view/components/system_management/message_center/approve/approve'
              )
          },
          {
            path: 'notify',
            name: 'notify',
            meta: {
              hideInMenu: true,
              title: '通知'
            },
            component: () =>
              import(
                '@/view/components/system_management/message_center/notify/notify'
              )
          },
          {
            path: 'feedback',
            name: 'feedback',
            meta: {
              hideInMenu: true,
              title: '反馈消息'
            },
            component: () =>
              import(
                '@/view/components/system_management/message_center/feedback/feedback'
              )
          },
          {
            path: 'publish',
            name: 'publish',
            meta: {
              hideInMenu: true,
              title: '发布消息'
            },
            component: () =>
              import(
                '@/view/components/system_management/message_center/publish/publish'
              )
          }
        ]
      }
    ]
  },
  // 监管日志
  {
    path: '/regulatory_management',
    name: 'regulatory_management',
    component: Main,
    meta: {
      icon: '_iconfont icon-zhongbaojiandu-feibenren',
      title: '监督管理'
    },
    children: [
      {
        path: 'outpatient_diary',
        name: 'outpatient_diary',
        meta: {
          icon: '_iconfont icon-menzhen',
          title: '门诊病历'
        },
        component: () =>
          import(
            '@/view/components/regulatory_management/outpatient_diary/outpatient_diary'
          )
      },
      {
        path: 'hospital_sense_control',
        name: 'hospital_sense_control',
        meta: {
          icon: '_iconfont icon-yuanganke-K',
          title: '院感控制'
        },
        component: () =>
          import(
            '@/view/components/regulatory_management/hospital_sense_control/hospital_sense_control'
          )
      },
      {
        path: 'regulatory_log',
        name: 'regulatory_log',
        redirect: { name: 'regulatory_log_notify' },
        meta: {
          icon: '_iconfont zhongbaojiandu-feibenren',
          title: '监管日志'
        },
        component: () =>
          import(
            '@/view/components/regulatory_management/regulatory_log/regulatory_log'
          ),
        children: [
          {
            path: 'regulatory_log_notify',
            name: 'regulatory_log_notify',
            meta: {
              hideInMenu: true,
              title: '监管日志'
            },
            component: () =>
              import(
                '@/view/components/regulatory_management/regulatory_log/notify/notify'
              )
          },
          {
            path: 'regulatory_log_publish',
            name: 'regulatory_log_publish',
            meta: {
              hideInMenu: true,
              title: '发布日志'
            },
            component: () =>
              import(
                '@/view/components/regulatory_management/regulatory_log/publish/publish'
              )
          }
        ]
      }
    ]
  },
  // 信息管理
  {
    path: '/information_management',
    name: 'information_management',
    component: Main,
    meta: {
      icon: 'ios-folder-outline',
      title: '信息管理'
    },
    children: [
      {
        path: 'repository',
        name: 'repository',
        meta: {
          icon: '_iconfont icon-zhishi',
          title: '知识库'
        },
        component: () =>
          import(
            '@/view/components/information_management/repository/repository'
          )
      },
      {
        path: 'institutional_repository',
        name: 'institutional_repository',
        meta: {
          icon: '_iconfont icon-zhiduguanli',
          title: '制度库'
        },
        component: () =>
          import(
            '@/view/components/information_management/institutional_repository/institutional_repository'
          )
      },
      {
        path: 'qualification_repository',
        name: 'qualification_repository',
        meta: {
          icon: '_iconfont icon-zizhi',
          title: '资质库'
        },
        component: () =>
          import(
            '@/view/components/information_management/qualification_repository/qualification_repository'
          )
      },
      {
        path: 'training_record',
        name: 'training_record',
        meta: {
          icon: '_iconfont icon-peixun',
          title: '培训记录'
        },
        component: () =>
          import(
            '@/view/components/information_management/training_record/training_record'
          )
      }
    ]
  },
  // 库房管理
  {
    path: '/warehouse_management',
    name: 'warehouse_management',
    component: Main,
    meta: {
      icon: 'ios-folder-outline',
      title: '库房管理'
    },
    children: [
      // {
      //   path: 'warehouse_menu',
      //   name: 'warehouse_menu',
      //   meta: {
      //     icon: '_iconfont icon-huanzheguanli',
      //     title: '库房目录'
      //   },
      //   component: () => import('@/view/components/warehouse_management/warehouse_menu/warehouse_menu')
      // }
    ]
  },
  // 基础档案
  {
    path: '/primary_info',
    name: 'primary_info',
    component: Main,
    meta: {
      icon: 'ios-folder-outline',
      title: '基础档案'
    },
    children: [
      {
        path: 'warehouse_menu',
        name: 'warehouse_menu',
        meta: {
          icon: '_iconfont icon-danganmuluchaxun',
          title: '档案目录'
        },
        component: () =>
          import('@/view/components/primary_info/warehouse_menu/warehouse_menu')
      },
      {
        path: 'medical_comparsion',
        name: 'medical_comparsion',
        meta: {
          icon: '_iconfont icon-medicaid-check',
          title: '医保对照'
        },
        component: parentView,
        children: [
          {
            path: 'drug',
            name: 'drug',
            meta: {
              title: '药品对照'
            },
            component: () =>
              import(
                '@/view/components/primary_info/medical_comparsion/drug/drug'
              )
          },
          {
            path: 'project',
            name: 'project',
            meta: {
              title: '其他对照'
            },
            component: () =>
              import(
                '@/view/components/primary_info/medical_comparsion/project/project'
              )
          },
          // {
          //   path: 'health_supplies',
          //   name: 'health_supplies',
          //   meta: {
          //     title: '卫生耗材对照'
          //   },
          //   component: () =>
          //     import(
          //       '@/view/components/primary_info/medical_comparsion/health_supplies/health_supplies'
          //     )
          // }
        ]
      },
      {
        path: 'dosage_form_catalogue',
        name: 'dosage_form_catalogue',
        meta: {
          icon: '_iconfont icon-jixingliebiao',
          title: '剂型目录'
        },
        component: () =>
          import(
            '@/view/components/primary_info/dosage_form_catalogue/dosage_form_catalogue'
          )
      },
      {
        path: 'units_catalog',
        name: 'units_catalog',
        meta: {
          icon: 'ios-menu',
          title: '单位目录'
        },
        component: () =>
          import('@/view/components/primary_info/units_catalog/units_catalog')
      },
      {
        path: 'goods_archives',
        name: 'goods_archives',
        meta: {
          icon: 'ios-pricetag-outline',
          title: '物品档案'
        },
        component: parentView,
        children: [
          {
            path: 'item_file_supplies',
            name: 'item_file_supplies',
            meta: {
              title: '卫生耗材'
            },
            component: () =>
              import(
                '@/view/components/primary_info/item_file/item_file_supplies'
              )
          },
          {
            path: 'item_file_drug',
            name: 'item_file_drug',
            meta: {
              title: '药品档案'
            },
            component: () =>
              import('@/view/components/primary_info/item_file/item_file_drug')
          },
          {
            path: 'fixed_assets',
            name: 'fixed_assets',
            meta: {
              title: '固定资产'
            },
            component: () =>
              import(
                '@/view/components/primary_info/item_file/item_file_fixed_assets'
              )
          },
          {
            path: 'low_value_consumable_products',
            name: 'low_value_consumable_products',
            meta: {
              title: '低值易耗品'
            },
            component: () =>
              import('@/view/components/primary_info/item_file/item_file_low')
          },
          {
            path: 'diagnostic_programs',
            name: 'diagnostic_programs',
            meta: {
              title: '诊疗项目'
            },
            component: () =>
              import(
                '@/view/components/primary_info/item_file/item_file_treatment_project'
              )
          }
        ]
      },
      {
        path: 'supplier',
        name: 'supplier',
        meta: {
          icon: '_iconfont icon-mechanism',
          title: '供应商'
        },
        component: () =>
          import('@/view/components/primary_info/supplier/supplier')
      }
    ]
  },
  // 打印
  {
    path: '/print',
    name: 'print',
    meta: {
      icon: 'iconfont icon-mulushezhi',
      title: '打印'
    },
    component: Print,
    children: [
      {
        path: 'purchase_print',
        name: 'purchase_print',
        meta: {
          title: '采购申请单'
        },
        component: () => import('@/view/print/components/purchase_print.vue')
      },
      {
        path: 'cost_summary',
        name: 'cost_summary',
        meta: {
          title: '收入总汇表等'
        },
        component: () => import('@/view/print/components/cost_summary.vue')
      },
      {
        path: 'income_daily/:params/:title',
        name: 'income_daily',
        meta: {
          title: '收入日报表'
        },
        component: () => import('@/view/print/components/income_daily.vue')
      },
      {
        path: 'profit_analysis',
        name: 'profit_analysis',
        meta: {
          title: '盈利分析'
        },
        component: () => import('@/view/print/components/profit_analysis.vue')
      },
      {
        path: 'cost_table',
        name: 'cost_table',
        meta: {
          title: '成本总汇表'
        },
        component: () => import('@/view/print/components/cost_table.vue')
      },
      {
        path: 'statistic_analysis',
        name: 'statistic_analysis',
        meta: {
          title: '统计分析'
        },
        component: () =>
          import('@/view/print/components/statistic_analysis.vue')
      },
      {
        path: 'material_statement',
        name: 'material_statement',
        meta: {
          title: '物资报表'
        },
        component: () =>
          import('@/view/print/components/material_statement.vue')
      },
      {
        path: 'material_statistics',
        name: 'material_statistics',
        meta: {
          title: '物资统计'
        },
        component: () =>
          import('@/view/print/components/material_statistics.vue')
      }
    ]
  },
  {
    path: '/401',
    name: 'error_401',
    meta: {
      hideInMenu: true
    },
    component: () => import('@/view/error-page/401')
  },
  {
    path: '/500',
    name: 'error_500',
    meta: {
      hideInMenu: true
    },
    component: () => import('@/view/error-page/500')
  },
  {
    path: '*',
    name: 'error_404',
    meta: {
      hideInMenu: true
    },
    component: () => import('@/view/error-page/404')
  }
]
