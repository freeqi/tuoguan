<style lang="less">
  @import './login.less';
</style>

<template>
  <div class="login">
    <div class="login-header">
      <img src="../../assets/images/logo-login.png" height="70" alt="">
      <div class="login-header-con">
        <h3>连锁血透中心管理信息系统</h3>
        <p>Hemodialysis Center Management System</p>
      </div>
    </div>
    <div class="login-con">
      <aside></aside>
      <main>
        <div class="form-con">
          <h4>欢迎登录</h4>
          <login-form @on-success-valid="handleSubmit" class="login-form"></login-form>
        </div>
      </main>
    </div>
    <p class="login-footer">2001-{{(new Date()).getFullYear()}} &copy; SWS重庆山外山血液净化技术股份有限公司</p>
    <div class="login-bg"></div>
  </div>
</template>

<script>
import LoginForm from '_c/login-form'
import { mapActions } from 'vuex'
export default {
  components: {
    LoginForm
  },
  methods: {
    ...mapActions([
      'handleLogin',
      'getUserInfo'
    ]),
    handleSubmit ({ userName, password }) {
      this.handleLogin({ userName, password }).then(res => {
        this.getUserInfo().then(res => {
          this.$router.push({
            name: 'home'
          })
        })
      })
    }
  }
}
</script>

<style>

</style>
