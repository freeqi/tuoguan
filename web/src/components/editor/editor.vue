<template>
  <div class="editor-wrapper">
    <div :id="editorId"></div>
  </div>
</template>

<script>
import Editor from 'wangeditor'
import 'wangeditor/release/wangEditor.min.css'
import { oneOf } from '@/libs/tools'
export default {
  name: 'Editor',
  props: {
    value: {
      type: String,
      default: ''
    },
    /**
     * 绑定的值的类型, enum: ['html', 'text']
     */
    valueType: {
      type: String,
      default: 'html',
      validator: (val) => {
        return oneOf(val, ['html', 'text'])
      }
    },
    /**
     * @description 设置change事件触发时间间隔
     */
    changeInterval: {
      type: Number,
      default: 200
    },
    /**
     * @description 是否开启本地存储
     */
    cache: {
      type: Boolean,
      default: true
    }
  },
  data () {
    return {
      menu: [
        'head', // 标题
        'bold', // 粗体
        'fontSize', // 字号
        'fontName', // 字体
        'italic', // 斜体
        'underline', // 下划线
        'strikeThrough', // 删除线
        // 'foreColor', // 文字颜色
        // 'backColor', // 背景颜色
        'link', // 插入链接
        'list', // 列表
        'justify', // 对齐方式
        'quote', // 引用
        'image' // 插入图片
        // 'emoticon',  // 表情
        // 'table',  // 表格
        // 'video',  // 插入视频
        // 'code',  // 插入代码
        // 'undo', // 撤销
        // 'redo' // 重复
      ]
    }
  },
  computed: {
    editorId () {
      return `editor${this._uid}`
    }
  },
  methods: {
    setHtml (val) {
      this.editor.txt.html(val)
    },
    // 获取editor编辑器内容
    getHtml () {
      return this.editor.txt.html()
    }
  },
  mounted () {
    this.editor = new Editor(`#${this.editorId}`)
    this.editor.customConfig.menus = this.menu
    this.editor.customConfig.onchange = (html) => {
      // let text = this.editor.txt.text()
      if (this.cache) localStorage.editorCache = html
      // this.$emit('input', this.valueType === 'html' ? html : text)
      // this.$emit('on-change', html, text)
    }
    this.editor.customConfig.onchangeTimeout = this.changeInterval
    // 上传图片
    // this.editor.customConfig.uploadImgShowBase64 = true // 使用 base64 保存图片
    // this.editor.customConfig.uploadImgServer = this.$config.baseURL.dev // 需要设置服务器上传地址

    // create这个方法一定要在所有配置项之后调用
    this.editor.create()
    // 如果本地有存储加载本地存储内容
    let html = this.value || localStorage.editorCache
    if (html) this.editor.txt.html(html)
  }

}
</script>

<style>

</style>
