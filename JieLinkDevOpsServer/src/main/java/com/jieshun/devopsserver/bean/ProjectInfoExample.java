package com.jieshun.devopsserver.bean;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class ProjectInfoExample {
    protected String orderByClause;

    protected boolean distinct;

    protected List<Criteria> oredCriteria;

    public ProjectInfoExample() {
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

        public Criteria andDevopsVersionIsNull() {
            addCriterion("devops_version is null");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionIsNotNull() {
            addCriterion("devops_version is not null");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionEqualTo(String value) {
            addCriterion("devops_version =", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionNotEqualTo(String value) {
            addCriterion("devops_version <>", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionGreaterThan(String value) {
            addCriterion("devops_version >", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionGreaterThanOrEqualTo(String value) {
            addCriterion("devops_version >=", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionLessThan(String value) {
            addCriterion("devops_version <", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionLessThanOrEqualTo(String value) {
            addCriterion("devops_version <=", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionLike(String value) {
            addCriterion("devops_version like", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionNotLike(String value) {
            addCriterion("devops_version not like", value, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionIn(List<String> values) {
            addCriterion("devops_version in", values, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionNotIn(List<String> values) {
            addCriterion("devops_version not in", values, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionBetween(String value1, String value2) {
            addCriterion("devops_version between", value1, value2, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andDevopsVersionNotBetween(String value1, String value2) {
            addCriterion("devops_version not between", value1, value2, "devopsVersion");
            return (Criteria) this;
        }

        public Criteria andIsFilterIsNull() {
            addCriterion("is_filter is null");
            return (Criteria) this;
        }

        public Criteria andIsFilterIsNotNull() {
            addCriterion("is_filter is not null");
            return (Criteria) this;
        }

        public Criteria andIsFilterEqualTo(Integer value) {
            addCriterion("is_filter =", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterNotEqualTo(Integer value) {
            addCriterion("is_filter <>", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterGreaterThan(Integer value) {
            addCriterion("is_filter >", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterGreaterThanOrEqualTo(Integer value) {
            addCriterion("is_filter >=", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterLessThan(Integer value) {
            addCriterion("is_filter <", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterLessThanOrEqualTo(Integer value) {
            addCriterion("is_filter <=", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterIn(List<Integer> values) {
            addCriterion("is_filter in", values, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterNotIn(List<Integer> values) {
            addCriterion("is_filter not in", values, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterBetween(Integer value1, Integer value2) {
            addCriterion("is_filter between", value1, value2, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterNotBetween(Integer value1, Integer value2) {
            addCriterion("is_filter not between", value1, value2, "isFilter");
            return (Criteria) this;
        }

        public Criteria andOperatorDateIsNull() {
            addCriterion("operator_date is null");
            return (Criteria) this;
        }

        public Criteria andOperatorDateIsNotNull() {
            addCriterion("operator_date is not null");
            return (Criteria) this;
        }

        public Criteria andOperatorDateEqualTo(Date value) {
            addCriterion("operator_date =", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateNotEqualTo(Date value) {
            addCriterion("operator_date <>", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateGreaterThan(Date value) {
            addCriterion("operator_date >", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateGreaterThanOrEqualTo(Date value) {
            addCriterion("operator_date >=", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateLessThan(Date value) {
            addCriterion("operator_date <", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateLessThanOrEqualTo(Date value) {
            addCriterion("operator_date <=", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateIn(List<Date> values) {
            addCriterion("operator_date in", values, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateNotIn(List<Date> values) {
            addCriterion("operator_date not in", values, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateBetween(Date value1, Date value2) {
            addCriterion("operator_date between", value1, value2, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateNotBetween(Date value1, Date value2) {
            addCriterion("operator_date not between", value1, value2, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andRemarkIsNull() {
            addCriterion("remark is null");
            return (Criteria) this;
        }

        public Criteria andRemarkIsNotNull() {
            addCriterion("remark is not null");
            return (Criteria) this;
        }

        public Criteria andRemarkEqualTo(String value) {
            addCriterion("remark =", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkNotEqualTo(String value) {
            addCriterion("remark <>", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkGreaterThan(String value) {
            addCriterion("remark >", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkGreaterThanOrEqualTo(String value) {
            addCriterion("remark >=", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkLessThan(String value) {
            addCriterion("remark <", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkLessThanOrEqualTo(String value) {
            addCriterion("remark <=", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkLike(String value) {
            addCriterion("remark like", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkNotLike(String value) {
            addCriterion("remark not like", value, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkIn(List<String> values) {
            addCriterion("remark in", values, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkNotIn(List<String> values) {
            addCriterion("remark not in", values, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkBetween(String value1, String value2) {
            addCriterion("remark between", value1, value2, "remark");
            return (Criteria) this;
        }

        public Criteria andRemarkNotBetween(String value1, String value2) {
            addCriterion("remark not between", value1, value2, "remark");
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