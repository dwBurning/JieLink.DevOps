package com.jieshun.devopsserver.bean;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import com.alibaba.fastjson.annotation.JSONField;

public class SysUser implements UserDetails {
    private Integer id;

    private String userName;

    private String password;

    private String cellPhone;

    private String email;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName == null ? null : userName.trim();
    }

    public void setPassword(String password) {
        this.password = password == null ? null : password.trim();
    }

    public String getCellPhone() {
        return cellPhone;
    }

    public void setCellPhone(String cellPhone) {
        this.cellPhone = cellPhone == null ? null : cellPhone.trim();
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email == null ? null : email.trim();
    }
    
 // 以下内容为 重写父类

 	@Override
 	@JSONField(serialize = false)
 	public Collection<? extends GrantedAuthority> getAuthorities() {
 		// TODO Auto-generated method stub
 		List<GrantedAuthority> authorities = new ArrayList<>();
 		// 暂时不做复杂的权限控制 所有用户都具备admin权限
 		authorities.add(new SimpleGrantedAuthority("ROLE_" + "admin"));
 		return authorities;
 	}

 	@Override
 	@JSONField(serialize = false)
 	public String getPassword() {
 		// TODO Auto-generated method stub
 		return password;
 	}

 	@Override
 	@JSONField(serialize = false)
 	public String getUsername() {
 		// TODO Auto-generated method stub
 		return userName;
 	}

 	@Override
 	@JSONField(serialize = false)
 	public boolean isAccountNonExpired() {
 		// TODO Auto-generated method stub
 		return true;
 	}

 	@Override
 	@JSONField(serialize = false)
 	public boolean isAccountNonLocked() {
 		// TODO Auto-generated method stub
 		return true;
 	}

 	@Override
 	@JSONField(serialize = false)
 	public boolean isCredentialsNonExpired() {
 		// TODO Auto-generated method stub
 		return true;
 	}

 	@Override
 	@JSONField(serialize = false)
 	public boolean isEnabled() {
 		// TODO Auto-generated method stub
 		return true;
 	}
}