package com.jieshun.devopsserver.bean;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class VerifyBillInfoExample {
    protected String orderByClause;

    protected boolean distinct;

    protected List<Criteria> oredCriteria;

    public VerifyBillInfoExample() {
        oredCriteria = new ArrayList<Criteria>();
    }

    public void setOrderByClause(String orderByClause) {
        this.orderByClause = orderByClause;
    }

    public String getOrderByClause() {
        return orderByClause;
    }

    public void setDistinct(boolean distinct) {
        this.distinct = distinct;
    }

    public boolean isDistinct() {
        return distinct;
    }

    public List<Criteria> getOredCriteria() {
        return oredCriteria;
    }

    public void or(Criteria criteria) {
        oredCriteria.add(criteria);
    }

    public Criteria or() {
        Criteria criteria = createCriteriaInternal();
        oredCriteria.add(criteria);
        return criteria;
    }

    public Criteria createCriteria() {
        Criteria criteria = createCriteriaInternal();
        if (oredCriteria.size() == 0) {
            oredCriteria.add(criteria);
        }
        return criteria;
    }

    protected Criteria createCriteriaInternal() {
        Criteria criteria = new Criteria();
        return criteria;
    }

    public void clear() {
        oredCriteria.clear();
        orderByClause = null;
        distinct = false;
    }

    protected abstract static class GeneratedCriteria {
        protected List<Criterion> criteria;

        protected GeneratedCriteria() {
            super();
            criteria = new ArrayList<Criterion>();
        }

        public boolean isValid() {
            return criteria.size() > 0;
        }

        public List<Criterion> getAllCriteria() {
            return criteria;
        }

        public List<Criterion> getCriteria() {
            return criteria;
        }

        protected void addCriterion(String condition) {
            if (condition == null) {
                throw new RuntimeException("Value for condition cannot be null");
            }
            criteria.add(new Criterion(condition));
        }

        protected void addCriterion(String condition, Object value, String property) {
            if (value == null) {
                throw new RuntimeException("Value for " + property + " cannot be null");
            }
            criteria.add(new Criterion(condition, value));
        }

        protected void addCriterion(String condition, Object value1, Object value2, String property) {
            if (value1 == null || value2 == null) {
                throw new RuntimeException("Between values for " + property + " cannot be null");
            }
            criteria.add(new Criterion(condition, value1, value2));
        }

        public Criteria andIdIsNull() {
            addCriterion("id is null");
            return (Criteria) this;
        }

        public Criteria andIdIsNotNull() {
            addCriterion("id is not null");
            return (Criteria) this;
        }

        public Criteria andIdEqualTo(Integer value) {
            addCriterion("id =", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotEqualTo(Integer value) {
            addCriterion("id <>", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdGreaterThan(Integer value) {
            addCriterion("id >", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdGreaterThanOrEqualTo(Integer value) {
            addCriterion("id >=", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdLessThan(Integer value) {
            addCriterion("id <", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdLessThanOrEqualTo(Integer value) {
            addCriterion("id <=", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdIn(List<Integer> values) {
            addCriterion("id in", values, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotIn(List<Integer> values) {
            addCriterion("id not in", values, "id");
            return (Criteria) this;
        }

        public Criteria andIdBetween(Integer value1, Integer value2) {
            addCriterion("id between", value1, value2, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotBetween(Integer value1, Integer value2) {
            addCriterion("id not between", value1, value2, "id");
            return (Criteria) this;
        }

        public Criteria andPublisherNameIsNull() {
            addCriterion("publisher_name is null");
            return (Criteria) this;
        }

        public Criteria andPublisherNameIsNotNull() {
            addCriterion("publisher_name is not null");
            return (Criteria) this;
        }

        public Criteria andPublisherNameEqualTo(String value) {
            addCriterion("publisher_name =", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameNotEqualTo(String value) {
            addCriterion("publisher_name <>", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameGreaterThan(String value) {
            addCriterion("publisher_name >", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameGreaterThanOrEqualTo(String value) {
            addCriterion("publisher_name >=", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameLessThan(String value) {
            addCriterion("publisher_name <", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameLessThanOrEqualTo(String value) {
            addCriterion("publisher_name <=", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameLike(String value) {
            addCriterion("publisher_name like", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameNotLike(String value) {
            addCriterion("publisher_name not like", value, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameIn(List<String> values) {
            addCriterion("publisher_name in", values, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameNotIn(List<String> values) {
            addCriterion("publisher_name not in", values, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameBetween(String value1, String value2) {
            addCriterion("publisher_name between", value1, value2, "publisherName");
            return (Criteria) this;
        }

        public Criteria andPublisherNameNotBetween(String value1, String value2) {
            addCriterion("publisher_name not between", value1, value2, "publisherName");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoIsNull() {
            addCriterion("project_shoper_no is null");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoIsNotNull() {
            addCriterion("project_shoper_no is not null");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoEqualTo(String value) {
            addCriterion("project_shoper_no =", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoNotEqualTo(String value) {
            addCriterion("project_shoper_no <>", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoGreaterThan(String value) {
            addCriterion("project_shoper_no >", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoGreaterThanOrEqualTo(String value) {
            addCriterion("project_shoper_no >=", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoLessThan(String value) {
            addCriterion("project_shoper_no <", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoLessThanOrEqualTo(String value) {
            addCriterion("project_shoper_no <=", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoLike(String value) {
            addCriterion("project_shoper_no like", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoNotLike(String value) {
            addCriterion("project_shoper_no not like", value, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoIn(List<String> values) {
            addCriterion("project_shoper_no in", values, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoNotIn(List<String> values) {
            addCriterion("project_shoper_no not in", values, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoBetween(String value1, String value2) {
            addCriterion("project_shoper_no between", value1, value2, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectShoperNoNotBetween(String value1, String value2) {
            addCriterion("project_shoper_no not between", value1, value2, "projectShoperNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoIsNull() {
            addCriterion("project_no is null");
            return (Criteria) this;
        }

        public Criteria andProjectNoIsNotNull() {
            addCriterion("project_no is not null");
            return (Criteria) this;
        }

        public Criteria andProjectNoEqualTo(String value) {
            addCriterion("project_no =", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoNotEqualTo(String value) {
            addCriterion("project_no <>", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoGreaterThan(String value) {
            addCriterion("project_no >", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoGreaterThanOrEqualTo(String value) {
            addCriterion("project_no >=", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoLessThan(String value) {
            addCriterion("project_no <", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoLessThanOrEqualTo(String value) {
            addCriterion("project_no <=", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoLike(String value) {
            addCriterion("project_no like", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoNotLike(String value) {
            addCriterion("project_no not like", value, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoIn(List<String> values) {
            addCriterion("project_no in", values, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoNotIn(List<String> values) {
            addCriterion("project_no not in", values, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoBetween(String value1, String value2) {
            addCriterion("project_no between", value1, value2, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNoNotBetween(String value1, String value2) {
            addCriterion("project_no not between", value1, value2, "projectNo");
            return (Criteria) this;
        }

        public Criteria andProjectNameIsNull() {
            addCriterion("project_name is null");
            return (Criteria) this;
        }

        public Criteria andProjectNameIsNotNull() {
            addCriterion("project_name is not null");
            return (Criteria) this;
        }

        public Criteria andProjectNameEqualTo(String value) {
            addCriterion("project_name =", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameNotEqualTo(String value) {
            addCriterion("project_name <>", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameGreaterThan(String value) {
            addCriterion("project_name >", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameGreaterThanOrEqualTo(String value) {
            addCriterion("project_name >=", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameLessThan(String value) {
            addCriterion("project_name <", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameLessThanOrEqualTo(String value) {
            addCriterion("project_name <=", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameLike(String value) {
            addCriterion("project_name like", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameNotLike(String value) {
            addCriterion("project_name not like", value, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameIn(List<String> values) {
            addCriterion("project_name in", values, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameNotIn(List<String> values) {
            addCriterion("project_name not in", values, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameBetween(String value1, String value2) {
            addCriterion("project_name between", value1, value2, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectNameNotBetween(String value1, String value2) {
            addCriterion("project_name not between", value1, value2, "projectName");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionIsNull() {
            addCriterion("project_big_version is null");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionIsNotNull() {
            addCriterion("project_big_version is not null");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionEqualTo(String value) {
            addCriterion("project_big_version =", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionNotEqualTo(String value) {
            addCriterion("project_big_version <>", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionGreaterThan(String value) {
            addCriterion("project_big_version >", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionGreaterThanOrEqualTo(String value) {
            addCriterion("project_big_version >=", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionLessThan(String value) {
            addCriterion("project_big_version <", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionLessThanOrEqualTo(String value) {
            addCriterion("project_big_version <=", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionLike(String value) {
            addCriterion("project_big_version like", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionNotLike(String value) {
            addCriterion("project_big_version not like", value, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionIn(List<String> values) {
            addCriterion("project_big_version in", values, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionNotIn(List<String> values) {
            addCriterion("project_big_version not in", values, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionBetween(String value1, String value2) {
            addCriterion("project_big_version between", value1, value2, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectBigVersionNotBetween(String value1, String value2) {
            addCriterion("project_big_version not between", value1, value2, "projectBigVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionIsNull() {
            addCriterion("project_version is null");
            return (Criteria) this;
        }

        public Criteria andProjectVersionIsNotNull() {
            addCriterion("project_version is not null");
            return (Criteria) this;
        }

        public Criteria andProjectVersionEqualTo(String value) {
            addCriterion("project_version =", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionNotEqualTo(String value) {
            addCriterion("project_version <>", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionGreaterThan(String value) {
            addCriterion("project_version >", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionGreaterThanOrEqualTo(String value) {
            addCriterion("project_version >=", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionLessThan(String value) {
            addCriterion("project_version <", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionLessThanOrEqualTo(String value) {
            addCriterion("project_version <=", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionLike(String value) {
            addCriterion("project_version like", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionNotLike(String value) {
            addCriterion("project_version not like", value, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionIn(List<String> values) {
            addCriterion("project_version in", values, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionNotIn(List<String> values) {
            addCriterion("project_version not in", values, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionBetween(String value1, String value2) {
            addCriterion("project_version between", value1, value2, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectVersionNotBetween(String value1, String value2) {
            addCriterion("project_version not between", value1, value2, "projectVersion");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardIsNull() {
            addCriterion("project_is_nonstandard is null");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardIsNotNull() {
            addCriterion("project_is_nonstandard is not null");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardEqualTo(Integer value) {
            addCriterion("project_is_nonstandard =", value, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardNotEqualTo(Integer value) {
            addCriterion("project_is_nonstandard <>", value, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardGreaterThan(Integer value) {
            addCriterion("project_is_nonstandard >", value, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardGreaterThanOrEqualTo(Integer value) {
            addCriterion("project_is_nonstandard >=", value, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardLessThan(Integer value) {
            addCriterion("project_is_nonstandard <", value, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardLessThanOrEqualTo(Integer value) {
            addCriterion("project_is_nonstandard <=", value, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardIn(List<Integer> values) {
            addCriterion("project_is_nonstandard in", values, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardNotIn(List<Integer> values) {
            addCriterion("project_is_nonstandard not in", values, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardBetween(Integer value1, Integer value2) {
            addCriterion("project_is_nonstandard between", value1, value2, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectIsNonstandardNotBetween(Integer value1, Integer value2) {
            addCriterion("project_is_nonstandard not between", value1, value2, "projectIsNonstandard");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteIsNull() {
            addCriterion("project_remote is null");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteIsNotNull() {
            addCriterion("project_remote is not null");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteEqualTo(String value) {
            addCriterion("project_remote =", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteNotEqualTo(String value) {
            addCriterion("project_remote <>", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteGreaterThan(String value) {
            addCriterion("project_remote >", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteGreaterThanOrEqualTo(String value) {
            addCriterion("project_remote >=", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteLessThan(String value) {
            addCriterion("project_remote <", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteLessThanOrEqualTo(String value) {
            addCriterion("project_remote <=", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteLike(String value) {
            addCriterion("project_remote like", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteNotLike(String value) {
            addCriterion("project_remote not like", value, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteIn(List<String> values) {
            addCriterion("project_remote in", values, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteNotIn(List<String> values) {
            addCriterion("project_remote not in", values, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteBetween(String value1, String value2) {
            addCriterion("project_remote between", value1, value2, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemoteNotBetween(String value1, String value2) {
            addCriterion("project_remote not between", value1, value2, "projectRemote");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordIsNull() {
            addCriterion("project_remote_password is null");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordIsNotNull() {
            addCriterion("project_remote_password is not null");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordEqualTo(String value) {
            addCriterion("project_remote_password =", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordNotEqualTo(String value) {
            addCriterion("project_remote_password <>", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordGreaterThan(String value) {
            addCriterion("project_remote_password >", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordGreaterThanOrEqualTo(String value) {
            addCriterion("project_remote_password >=", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordLessThan(String value) {
            addCriterion("project_remote_password <", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordLessThanOrEqualTo(String value) {
            addCriterion("project_remote_password <=", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordLike(String value) {
            addCriterion("project_remote_password like", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordNotLike(String value) {
            addCriterion("project_remote_password not like", value, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordIn(List<String> values) {
            addCriterion("project_remote_password in", values, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordNotIn(List<String> values) {
            addCriterion("project_remote_password not in", values, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordBetween(String value1, String value2) {
            addCriterion("project_remote_password between", value1, value2, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemotePasswordNotBetween(String value1, String value2) {
            addCriterion("project_remote_password not between", value1, value2, "projectRemotePassword");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkIsNull() {
            addCriterion("project_remark is null");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkIsNotNull() {
            addCriterion("project_remark is not null");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkEqualTo(String value) {
            addCriterion("project_remark =", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkNotEqualTo(String value) {
            addCriterion("project_remark <>", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkGreaterThan(String value) {
            addCriterion("project_remark >", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkGreaterThanOrEqualTo(String value) {
            addCriterion("project_remark >=", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkLessThan(String value) {
            addCriterion("project_remark <", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkLessThanOrEqualTo(String value) {
            addCriterion("project_remark <=", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkLike(String value) {
            addCriterion("project_remark like", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkNotLike(String value) {
            addCriterion("project_remark not like", value, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkIn(List<String> values) {
            addCriterion("project_remark in", values, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkNotIn(List<String> values) {
            addCriterion("project_remark not in", values, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkBetween(String value1, String value2) {
            addCriterion("project_remark between", value1, value2, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectRemarkNotBetween(String value1, String value2) {
            addCriterion("project_remark not between", value1, value2, "projectRemark");
            return (Criteria) this;
        }

        public Criteria andProjectTaskIsNull() {
            addCriterion("project_task is null");
            return (Criteria) this;
        }

        public Criteria andProjectTaskIsNotNull() {
            addCriterion("project_task is not null");
            return (Criteria) this;
        }

        public Criteria andProjectTaskEqualTo(String value) {
            addCriterion("project_task =", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskNotEqualTo(String value) {
            addCriterion("project_task <>", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskGreaterThan(String value) {
            addCriterion("project_task >", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskGreaterThanOrEqualTo(String value) {
            addCriterion("project_task >=", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskLessThan(String value) {
            addCriterion("project_task <", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskLessThanOrEqualTo(String value) {
            addCriterion("project_task <=", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskLike(String value) {
            addCriterion("project_task like", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskNotLike(String value) {
            addCriterion("project_task not like", value, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskIn(List<String> values) {
            addCriterion("project_task in", values, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskNotIn(List<String> values) {
            addCriterion("project_task not in", values, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskBetween(String value1, String value2) {
            addCriterion("project_task between", value1, value2, "projectTask");
            return (Criteria) this;
        }

        public Criteria andProjectTaskNotBetween(String value1, String value2) {
            addCriterion("project_task not between", value1, value2, "projectTask");
            return (Criteria) this;
        }

        public Criteria andAddDateIsNull() {
            addCriterion("add_date is null");
            return (Criteria) this;
        }

        public Criteria andAddDateIsNotNull() {
            addCriterion("add_date is not null");
            return (Criteria) this;
        }

        public Criteria andAddDateEqualTo(Date value) {
            addCriterion("add_date =", value, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateNotEqualTo(Date value) {
            addCriterion("add_date <>", value, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateGreaterThan(Date value) {
            addCriterion("add_date >", value, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateGreaterThanOrEqualTo(Date value) {
            addCriterion("add_date >=", value, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateLessThan(Date value) {
            addCriterion("add_date <", value, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateLessThanOrEqualTo(Date value) {
            addCriterion("add_date <=", value, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateIn(List<Date> values) {
            addCriterion("add_date in", values, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateNotIn(List<Date> values) {
            addCriterion("add_date not in", values, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateBetween(Date value1, Date value2) {
            addCriterion("add_date between", value1, value2, "addDate");
            return (Criteria) this;
        }

        public Criteria andAddDateNotBetween(Date value1, Date value2) {
            addCriterion("add_date not between", value1, value2, "addDate");
            return (Criteria) this;
        }

        public Criteria andStatusIsNull() {
            addCriterion("status is null");
            return (Criteria) this;
        }

        public Criteria andStatusIsNotNull() {
            addCriterion("status is not null");
            return (Criteria) this;
        }

        public Criteria andStatusEqualTo(Integer value) {
            addCriterion("status =", value, "status");
            return (Criteria) this;
        }

        public Criteria andStatusNotEqualTo(Integer value) {
            addCriterion("status <>", value, "status");
            return (Criteria) this;
        }

        public Criteria andStatusGreaterThan(Integer value) {
            addCriterion("status >", value, "status");
            return (Criteria) this;
        }

        public Criteria andStatusGreaterThanOrEqualTo(Integer value) {
            addCriterion("status >=", value, "status");
            return (Criteria) this;
        }

        public Criteria andStatusLessThan(Integer value) {
            addCriterion("status <", value, "status");
            return (Criteria) this;
        }

        public Criteria andStatusLessThanOrEqualTo(Integer value) {
            addCriterion("status <=", value, "status");
            return (Criteria) this;
        }

        public Criteria andStatusIn(List<Integer> values) {
            addCriterion("status in", values, "status");
            return (Criteria) this;
        }

        public Criteria andStatusNotIn(List<Integer> values) {
            addCriterion("status not in", values, "status");
            return (Criteria) this;
        }

        public Criteria andStatusBetween(Integer value1, Integer value2) {
            addCriterion("status between", value1, value2, "status");
            return (Criteria) this;
        }

        public Criteria andStatusNotBetween(Integer value1, Integer value2) {
            addCriterion("status not between", value1, value2, "status");
            return (Criteria) this;
        }

        public Criteria andEmergencyIsNull() {
            addCriterion("emergency is null");
            return (Criteria) this;
        }

        public Criteria andEmergencyIsNotNull() {
            addCriterion("emergency is not null");
            return (Criteria) this;
        }

        public Criteria andEmergencyEqualTo(Integer value) {
            addCriterion("emergency =", value, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyNotEqualTo(Integer value) {
            addCriterion("emergency <>", value, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyGreaterThan(Integer value) {
            addCriterion("emergency >", value, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyGreaterThanOrEqualTo(Integer value) {
            addCriterion("emergency >=", value, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyLessThan(Integer value) {
            addCriterion("emergency <", value, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyLessThanOrEqualTo(Integer value) {
            addCriterion("emergency <=", value, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyIn(List<Integer> values) {
            addCriterion("emergency in", values, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyNotIn(List<Integer> values) {
            addCriterion("emergency not in", values, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyBetween(Integer value1, Integer value2) {
            addCriterion("emergency between", value1, value2, "emergency");
            return (Criteria) this;
        }

        public Criteria andEmergencyNotBetween(Integer value1, Integer value2) {
            addCriterion("emergency not between", value1, value2, "emergency");
            return (Criteria) this;
        }

        public Criteria andIsdeleteIsNull() {
            addCriterion("isdelete is null");
            return (Criteria) this;
        }

        public Criteria andIsdeleteIsNotNull() {
            addCriterion("isdelete is not null");
            return (Criteria) this;
        }

        public Criteria andIsdeleteEqualTo(Integer value) {
            addCriterion("isdelete =", value, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteNotEqualTo(Integer value) {
            addCriterion("isdelete <>", value, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteGreaterThan(Integer value) {
            addCriterion("isdelete >", value, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteGreaterThanOrEqualTo(Integer value) {
            addCriterion("isdelete >=", value, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteLessThan(Integer value) {
            addCriterion("isdelete <", value, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteLessThanOrEqualTo(Integer value) {
            addCriterion("isdelete <=", value, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteIn(List<Integer> values) {
            addCriterion("isdelete in", values, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteNotIn(List<Integer> values) {
            addCriterion("isdelete not in", values, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteBetween(Integer value1, Integer value2) {
            addCriterion("isdelete between", value1, value2, "isdelete");
            return (Criteria) this;
        }

        public Criteria andIsdeleteNotBetween(Integer value1, Integer value2) {
            addCriterion("isdelete not between", value1, value2, "isdelete");
            return (Criteria) this;
        }

        public Criteria andFinishDateIsNull() {
            addCriterion("finish_date is null");
            return (Criteria) this;
        }

        public Criteria andFinishDateIsNotNull() {
            addCriterion("finish_date is not null");
            return (Criteria) this;
        }

        public Criteria andFinishDateEqualTo(Date value) {
            addCriterion("finish_date =", value, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateNotEqualTo(Date value) {
            addCriterion("finish_date <>", value, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateGreaterThan(Date value) {
            addCriterion("finish_date >", value, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateGreaterThanOrEqualTo(Date value) {
            addCriterion("finish_date >=", value, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateLessThan(Date value) {
            addCriterion("finish_date <", value, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateLessThanOrEqualTo(Date value) {
            addCriterion("finish_date <=", value, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateIn(List<Date> values) {
            addCriterion("finish_date in", values, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateNotIn(List<Date> values) {
            addCriterion("finish_date not in", values, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateBetween(Date value1, Date value2) {
            addCriterion("finish_date between", value1, value2, "finishDate");
            return (Criteria) this;
        }

        public Criteria andFinishDateNotBetween(Date value1, Date value2) {
            addCriterion("finish_date not between", value1, value2, "finishDate");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameIsNull() {
            addCriterion("uploadfilename is null");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameIsNotNull() {
            addCriterion("uploadfilename is not null");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameEqualTo(String value) {
            addCriterion("uploadfilename =", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameNotEqualTo(String value) {
            addCriterion("uploadfilename <>", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameGreaterThan(String value) {
            addCriterion("uploadfilename >", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameGreaterThanOrEqualTo(String value) {
            addCriterion("uploadfilename >=", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameLessThan(String value) {
            addCriterion("uploadfilename <", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameLessThanOrEqualTo(String value) {
            addCriterion("uploadfilename <=", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameLike(String value) {
            addCriterion("uploadfilename like", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameNotLike(String value) {
            addCriterion("uploadfilename not like", value, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameIn(List<String> values) {
            addCriterion("uploadfilename in", values, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameNotIn(List<String> values) {
            addCriterion("uploadfilename not in", values, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameBetween(String value1, String value2) {
            addCriterion("uploadfilename between", value1, value2, "uploadfilename");
            return (Criteria) this;
        }

        public Criteria andUploadfilenameNotBetween(String value1, String value2) {
            addCriterion("uploadfilename not between", value1, value2, "uploadfilename");
            return (Criteria) this;
        }
    }

    public static class Criteria extends GeneratedCriteria {

        protected Criteria() {
            super();
        }
    }

    public static class Criterion {
        private String condition;

        private Object value;

        private Object secondValue;

        private boolean noValue;

        private boolean singleValue;

        private boolean betweenValue;

        private boolean listValue;

        private String typeHandler;

        public String getCondition() {
            return condition;
        }

        public Object getValue() {
            return value;
        }

        public Object getSecondValue() {
            return secondValue;
        }

        public boolean isNoValue() {
            return noValue;
        }

        public boolean isSingleValue() {
            return singleValue;
        }

        public boolean isBetweenValue() {
            return betweenValue;
        }

        public boolean isListValue() {
            return listValue;
        }

        public String getTypeHandler() {
            return typeHandler;
        }

        protected Criterion(String condition) {
            super();
            this.condition = condition;
            this.typeHandler = null;
            this.noValue = true;
        }

        protected Criterion(String condition, Object value, String typeHandler) {
            super();
            this.condition = condition;
            this.value = value;
            this.typeHandler = typeHandler;
            if (value instanceof List<?>) {
                this.listValue = true;
            } else {
                this.singleValue = true;
            }
        }

        protected Criterion(String condition, Object value) {
            this(condition, value, null);
        }

        protected Criterion(String condition, Object value, Object secondValue, String typeHandler) {
            super();
            this.condition = condition;
            this.value = value;
            this.secondValue = secondValue;
            this.typeHandler = typeHandler;
            this.betweenValue = true;
        }

        protected Criterion(String condition, Object value, Object secondValue) {
            this(condition, value, secondValue, null);
        }
    }
}